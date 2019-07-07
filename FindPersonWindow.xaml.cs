using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for FindPersonWindow.xaml
    /// </summary>
    public partial class FindPersonWindow : Window
    {
        public FindPersonWindow()
        {
            InitializeComponent();
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            WindowMain mw = new WindowMain();
            mw.Show();
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            string username = TextBox_Username.Text;
            if (String.IsNullOrEmpty(username))
            {
                MessageBox.Show("Fill username textbox");
            }
            else
            {
                listView.Items.Clear();
                UserRepo userRepo = UserRepo.Instance();
                var resultsList = userRepo.getSimiliarUsers(username);
                foreach (var result in resultsList)
                {
                    listView.Items.Add(result);
                }

            }
        }
    }
}
