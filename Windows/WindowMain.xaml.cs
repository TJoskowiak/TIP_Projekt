using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            InitializeComponent();

            loadContacts();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_FindPerson_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            FindPersonWindow fpw = new FindPersonWindow();
            fpw.Owner = this;
            fpw.Show();
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void loadContacts()
        {

            listBox.Items.Clear();
            ContactsRepo contRepo = ContactsRepo.Instance();
            UserRepo userRepo = UserRepo.Instance();

            var resultsList = contRepo.getAllCurrentContacts();
            resultsList = userRepo.getUsernamesByIds(resultsList);


            foreach (var result in resultsList)
            {
                listBox.Items.Add(result);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            UserRepo userRepo = UserRepo.Instance();
            userRepo.setUserOffline(Globals.currentUserLogin);
        }
    }
}
