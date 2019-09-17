using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VOiP_Communicator.Classes;
using System.Runtime.InteropServices;


namespace VOiP_Communicator
{
    public partial class LoginWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);



        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public LoginWindow()
        {
            InitializeComponent();
            IntPtr hWnd = FindWindow(null, Environment.SystemDirectory + "\\cmd.exe"); //put your console window caption here
            ShowWindow(hWnd, 0); // 0 = SW_HIDE
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            RegisterForm rf = new RegisterForm();
            rf.Owner = this;
            rf.Show();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if(validate(Login.Text, Password.Password))
            {
                UserRepo userRepo = UserRepo.Instance();

                userRepo.updateLogin(Login.Text, GetLocalIPAddress());
                WindowMain wm = new WindowMain();
                wm.Show();
                this.Close();

            }
        }

        private bool validate(string login, string password)
        {
            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))
            {
                MessageBox.Show("Fill in all fields");
                return false;
            }

            UserRepo userRepo = UserRepo.Instance();

            if (String.IsNullOrEmpty(userRepo.GetColumnValueByUsername(login, "username")))
            {
                MessageBox.Show("No such user in database");
                return false;
            }

            Tuple<string, string> hashAndPassword = userRepo.GetSaltAndPassowrdByUsername(login);

            if(!String.Equals(SHA512(password+hashAndPassword.Item1), hashAndPassword.Item2))
            {
                MessageBox.Show("Invalid password");
                return false;
            }

            Globals.currentUserLogin = userRepo.GetColumnValueByUsername(login, "username");
            Globals.currentUserId = System.Convert.ToInt32(userRepo.GetColumnValueByUsername(login, "user_id"));

            return true;
        }

        private string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        private string GetLocalIPAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
    }
}
