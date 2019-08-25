using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows;
using Microsoft.DirectX.DirectSound;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using g711audio;


namespace VOiP_Communicator.Classes
{

    #region Enumerations
    //The commands for interaction between the two parties.
    enum Command
    {
        Invite, //Make a call.
        Bye,    //End a call.
        Busy,   //User busy.
        OK,     //Response to an invite message. OK is send to indicate that call is accepted.
        Null,   //No command.
    }

    //Vocoder
    enum Vocoder
    {
        ALaw,   //A-Law vocoder.
        uLaw,   //u-Law vocoder.
        None,   //Don't use any vocoder.
    }
    #endregion


    public class CallManager
    {
        private CaptureBufferDescription captureBufferDescription;
        private AutoResetEvent autoResetEvent;
        private Notify notify;
        private WaveFormat waveFormat;
        private Capture capture;
        private int bufferSize;
        private UdpClient udpClient;                //Listens and sends data on port 1550, used in synchronous mode.
        private Device device;
        private CaptureBuffer captureBuffer;
        private SecondaryBuffer playbackBuffer;
        private BufferDescription playbackBufferDescription;
        private Socket clientSocket;
        private bool bStop;                         //Flag to end the Start and Receive threads.
        private IPEndPoint otherPartyIP;            //IP of party we want to make a call.
        private EndPoint otherPartyEP;
        private volatile bool bIsCallActive;                 //Tells whether we have an active call.
        private byte[] byteData = new byte[1024];   //Buffer to store the data received.
        private volatile int nUdpClientFlag;                 //Flag used to close the udpClient socket.
        private bool IsCaller = false;


        private DateTime CallDate;
        private int ReceiverID;

        /*
        * Initializes all the data members.
        */
       public CallManager(System.Windows.Window window)
       {
            try
            {
                device = new Device();
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(window);
                device.SetCooperativeLevel(helper.Handle, CooperativeLevel.Normal);

                CaptureDevicesCollection captureDeviceCollection = new CaptureDevicesCollection();

                DeviceInformation deviceInfo = captureDeviceCollection[0];

                capture = new Capture(deviceInfo.DriverGuid);

                short channels = 1; //Stereo.
                short bitsPerSample = 16; //16Bit, alternatively use 8Bits.
                int samplesPerSecond = 22050; //11KHz use 11025 , 22KHz use 22050, 44KHz use 44100 etc.

                //Set up the wave format to be captured.
                waveFormat = new WaveFormat();
                waveFormat.Channels = channels;
                waveFormat.FormatTag = WaveFormatTag.Pcm;
                waveFormat.SamplesPerSecond = samplesPerSecond;
                waveFormat.BitsPerSample = bitsPerSample;
                waveFormat.BlockAlign = (short)(channels * (bitsPerSample / (short)8));
                waveFormat.AverageBytesPerSecond = waveFormat.BlockAlign * samplesPerSecond;

                captureBufferDescription = new CaptureBufferDescription();
                captureBufferDescription.BufferBytes = waveFormat.AverageBytesPerSecond / 5;//approx 200 milliseconds of PCM data.
                captureBufferDescription.Format = waveFormat;

                playbackBufferDescription = new BufferDescription();
                playbackBufferDescription.BufferBytes = waveFormat.AverageBytesPerSecond / 5;
                playbackBufferDescription.Format = waveFormat;
                playbackBuffer = new SecondaryBuffer(playbackBufferDescription, device);

                bufferSize = captureBufferDescription.BufferBytes;

                bIsCallActive = false;
                nUdpClientFlag = 0;

                //Using UDP sockets
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                EndPoint ourEP = new IPEndPoint(IPAddress.Any, 1450);
                //Listen asynchronously on port 1450 for coming messages (Invite, Bye, etc).
                clientSocket.Bind(ourEP);

                //Receive data from any IP.
                EndPoint remoteEP = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

                byteData = new byte[1024];
                //Receive data asynchornously.
                clientSocket.BeginReceiveFrom(byteData,
                                           0, byteData.Length,
                                           SocketFlags.None,
                                           ref remoteEP,
                                           new AsyncCallback(OnReceive),
                                           null);
            }
            catch (Exception e)
            {
               
                MessageBox.Show(e.ToString(), "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        // Starts a call
        public void Call(string IP, int receiverID, int port = 1450)
        {
            CallDate = DateTime.Now;
            IsCaller = true;
            this.ReceiverID = receiverID;

            try
            {
                //Get the IP we want to call.
                otherPartyIP = new IPEndPoint(IPAddress.Parse(IP), port);
                otherPartyEP = (EndPoint)otherPartyIP;

                //Send an invite message.
                SendMessage(Command.Invite, otherPartyEP);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "VoiceChat-Call ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }


        private void OnSend(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSendTo(ar);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "VoiceChat-OnSend ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /*
         * Commands are received asynchronously. OnReceive is the handler for them.
         */
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                EndPoint receivedFromEP = new IPEndPoint(IPAddress.Any, 0);

                //Get the IP from where we got a message.
                clientSocket.EndReceiveFrom(ar, ref receivedFromEP);

                //Convert the bytes received into an object of type Data.
                Data msgReceived = new Data(byteData);

                //Act according to the received message.
                switch (msgReceived.cmdCommand)
                {
                    //We have an incoming call.
                    case Command.Invite:
                        {
                            if (bIsCallActive == false)
                            {
                                //We have no active call.

                                //Ask the user to accept the call or not.
                                if (MessageBox.Show("Call coming from " + msgReceived.strName + ".\r\n\r\nAccept it?",
                                    "VoiceChat", MessageBoxButton.YesNo, MessageBoxImage.Question,MessageBoxResult.Yes) == MessageBoxResult.Yes)
                                {
                                    SendMessage(Command.OK, receivedFromEP);
                                    otherPartyEP = receivedFromEP;
                                    otherPartyIP = (IPEndPoint)receivedFromEP;
                                    InitializeCall();
                                }
                                else
                                {
                                    //The call is declined. Send a busy response.
                                    SendMessage(Command.Busy, receivedFromEP);
                                }
                            }
                            else
                            {
                                //We already have an existing call. Send a busy response.
                                SendMessage(Command.Busy, receivedFromEP);
                            }
                            break;
                        }

                    //OK is received in response to an Invite.
                    case Command.OK:
                        {
                            //Start a call.
                            InitializeCall();
                            break;
                        }

                    //Remote party is busy.
                    case Command.Busy:
                        {
                            MessageBox.Show("User busy.", "VoiceChat", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                            CallRepo.createCall(Globals.currentUserId, ReceiverID, CallDate.ToString(), 5);
                            IsCaller = false;
                            break;
                        }

                    case Command.Bye:
                        {
                            //Check if the Bye command has indeed come from the user/IP with which we have
                            //a call established. This is used to prevent other users from sending a Bye, which
                            //would otherwise end the call.
                            if (receivedFromEP.Equals(otherPartyEP) == true)
                            {
                                //End the call.
                                UninitializeCall();
                            }
                            break;
                        }
                }

                byteData = new byte[1024];
                //Get ready to receive more commands.
                clientSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref receivedFromEP, new AsyncCallback(OnReceive), null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "VoiceChat-OnReceive ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /*
       * Send synchronously sends data captured from microphone across the network on port 1550.
       */
        private void Send()
        {
            
            try
            {
                captureBuffer = new CaptureBuffer(captureBufferDescription, capture);
                //The following lines get audio from microphone and then send them 
                //across network

                CreateNotifyPositions();

                int halfBuffer = bufferSize / 2;

                captureBuffer.Start(true);

                bool readFirstBufferPart = true;
                int offset = 0;

                MemoryStream memStream = new MemoryStream(halfBuffer);
                bStop = false;
                while (!bStop)
                {
                    autoResetEvent.WaitOne();
                    memStream.Seek(0, SeekOrigin.Begin);
                    captureBuffer.Read(offset, memStream, halfBuffer, LockFlag.None);
                    readFirstBufferPart = !readFirstBufferPart;
                    offset = readFirstBufferPart ? 0 : halfBuffer;

                    //TODO: Fix this ugly way of initializing differently.

                    //Choose the vocoder. And then send the data to other party at port 1550.
         
                    byte[] dataToWrite = ALawEncoder.ALawEncode(memStream.GetBuffer());
                    udpClient.Send(dataToWrite, dataToWrite.Length, otherPartyIP.Address.ToString(), 1550);
                   
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "VoiceChat-Send ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            finally
            {
                captureBuffer.Stop();

                //Increment flag by one.
                nUdpClientFlag += 1;

                //When flag is two then it means we have got out of loops in Send and Receive.
                while (nUdpClientFlag != 2)
                { }

                //Clear the flag.
                nUdpClientFlag = 0;

                //Close the socket.
                udpClient.Close();
            }
        }

        /*
         * Receive audio data coming on port 1550 and feed it to the speakers to be played.
         */
        private void Receive()
        {
            try
            {
                bStop = false;
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                while (!bStop)
                {
                    //Receive data.
                    byte[] byteData = udpClient.Receive(ref remoteEP);

                    //G711 compresses the data by 50%, so we allocate a buffer of double
                    //the size to store the decompressed data.
                    byte[] byteDecodedData = new byte[byteData.Length * 2];

                    //Decompress data using the proper vocoder.
                   
                    ALawDecoder.ALawDecode(byteData, out byteDecodedData);
                   
                    //Play the data received to the user.
                    playbackBuffer = new SecondaryBuffer(playbackBufferDescription, device);
                    playbackBuffer.Write(0, byteDecodedData, LockFlag.None);
                    playbackBuffer.Play(0, BufferPlayFlags.Default);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "VoiceChat-Receive ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            finally
            {
                nUdpClientFlag += 1;
            }
        }


        private void CreateNotifyPositions()
        {
            try
            {
                autoResetEvent = new AutoResetEvent(false);
                notify = new Notify(captureBuffer);
                BufferPositionNotify bufferPositionNotify1 = new BufferPositionNotify();
                bufferPositionNotify1.Offset = bufferSize / 2 - 1;
                bufferPositionNotify1.EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();
                BufferPositionNotify bufferPositionNotify2 = new BufferPositionNotify();
                bufferPositionNotify2.Offset = bufferSize - 1;
                bufferPositionNotify2.EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();

                notify.SetNotificationPositions(new BufferPositionNotify[] { bufferPositionNotify1, bufferPositionNotify2 });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "VoiceChat-CreateNotifyPositions ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }


        private void UninitializeCall()
        {
            //Set the flag to end the Send and Receive threads.
            bStop = true;
            bIsCallActive = false;
            
            
            if(IsCaller)
            {
                CallRepo.createCall(Globals.currentUserId, ReceiverID, CallDate.ToString(), 1);
            }
            clientSocket.Dispose();
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint ourEP = new IPEndPoint(IPAddress.Any, 1450);

            IsCaller = false;
            
        }

        public void DropCall()
        {
            try
            {
                //Send a Bye message to the user to end the call.
                SendMessage(Command.Bye, otherPartyEP);
                UninitializeCall();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "VoiceChat-DropCall ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void InitializeCall()
        {
            try
            {
                //Start listening on port 1500.
                udpClient = new UdpClient(1550);

                Thread senderThread = new Thread(new ThreadStart(Send));
                Thread receiverThread = new Thread(new ThreadStart(Receive));
                bIsCallActive = true;

                //Start the receiver and sender thread.
                receiverThread.Start();
                senderThread.Start();
                //btnCall.Enabled = false;
                //btnEndCall.Enabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "VoiceChat-InitializeCall ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /*
         * Send a message to the remote party.
         */
        private void SendMessage(Command cmd, EndPoint sendToEP)
        {
            try
            {
                //Create the message to send.
                Data msgToSend = new Data();

                msgToSend.strName = Globals.currentUserLogin;  //txtName.Text;   //Name of the user.
                msgToSend.cmdCommand = cmd;         //Message to send.
               

                byte[] message = msgToSend.ToByte();

                //Send the message asynchronously.
                clientSocket.BeginSendTo(message, 0, message.Length, SocketFlags.None, sendToEP, new AsyncCallback(OnSend), null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "VoiceChat-SendMessage ()", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

    }
}
