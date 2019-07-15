using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VOiP_Communicator
{
    /// <summary>
    /// Interaction logic for RegisterForm.xaml
    /// </summary>
    public partial class RegisterForm : Window
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void Back_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.Owner.IsEnabled = true;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string email = Email.Text;
            string password = Password.Password;
            string repeat_password = Retype_Password.Password;

            if (validate(username, email, password, repeat_password))
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[128];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                string salt = new string(stringChars);

                UserRepo userRepo = UserRepo.Instance();

                string passwordHash = SHA512(password + salt);

                userRepo.createUser(username, email, passwordHash, salt, GetLocalIPAddress(), 0);
                this.Close();
                MessageBox.Show("You have succesfully registered, now you can login");
                this.Owner.IsEnabled = true;
            }
            
        }

        private bool validate(string username, string email, string password, string retype_password)
        {
            if(String.IsNullOrEmpty(username) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(retype_password))
            {
                MessageBox.Show("Fill in all fields");
                return false;
            }

             UserRepo userRepo = UserRepo.Instance();

            if(!String.IsNullOrEmpty(userRepo.GetByUsername(username)))
            {
                MessageBox.Show("Username is taken");
                return false;
            }

            if(!String.Equals(password, retype_password))
            {
                MessageBox.Show("You typed 2 different passwords");
                return false;
            }

            if(!IsEmailValid(email))
            {
                MessageBox.Show("Invalid email format");
                return false;
            }

            return true;
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

        private bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
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
    }
}
