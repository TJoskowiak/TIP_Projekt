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

namespace VOiP_Communicator
{
    public partial class LoginWindow : Window
    {
        public LoginWindow() => InitializeComponent();

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterForm rf = new RegisterForm();
            rf.Show();
            this.Hide();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if(validate(Login.Text, Password.Password))
            {
                UserRepo userRepo = UserRepo.Instance();

                userRepo.updateLogin(Login.Text, GetLocalIPAddress());
                this.Hide();
                WindowMain wm = new WindowMain();
                wm.Show();
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

            if (String.IsNullOrEmpty(userRepo.GetByUsername(login)))
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
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
