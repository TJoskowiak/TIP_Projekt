using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOiP_Communicator.Classes
{
    class Data
    {
        //Default constructor.
        public Data()
        {
            this.cmdCommand = Command.Null;
            this.strName = null;
            vocoder = Vocoder.ALaw;
        }

        //Converts the bytes into an object of type Data.
        public Data(byte[] data)
        {
            //The first four bytes are for the Command.
            this.cmdCommand = (Command)BitConverter.ToInt32(data, 0);

            //The next four store the length of the name.
            int nameLen = BitConverter.ToInt32(data, 4);

            //This check makes sure that strName has been passed in the array of bytes.
            if (nameLen > 0)
                this.strName = Encoding.UTF8.GetString(data, 8, nameLen);
            else
                this.strName = null;
        }

        //Converts the Data structure into an array of bytes.
        public byte[] ToByte()
        {
            List<byte> result = new List<byte>();

            //First four are for the Command.
            result.AddRange(BitConverter.GetBytes((int)cmdCommand));

            //Add the length of the name.
            if (strName != null)
                result.AddRange(BitConverter.GetBytes(strName.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Add the name.
            if (strName != null)
                result.AddRange(Encoding.UTF8.GetBytes(strName));

            return result.ToArray();
        }

        public string strName;      //Name by which the client logs into the room.
        public Command cmdCommand;  //Command type (login, logout, send message, etc).
        public Vocoder vocoder;
    }
}
