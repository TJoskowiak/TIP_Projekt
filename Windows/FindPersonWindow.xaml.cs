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
            this.Close();
            this.Owner.IsEnabled = true;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var x = (SearchResult)listView.SelectedItem;
            if (x != null)
            {
                ContactsRepo contactRepo = ContactsRepo.Instance();
                UserRepo userRepo = UserRepo.Instance();

                int subject_id = Int32.Parse(userRepo.GetColumnValueByUsername(x.Username, "user_id"));
                int owner_id = Int32.Parse(userRepo.GetColumnValueByUsername(Globals.currentUserLogin, "user_id"));
                DateTime created_date = DateTime.Now;
                int is_favourite = 0;

                if (contactRepo.contactExists(owner_id, subject_id))
                {
                    MessageBox.Show("You already have this user in your contacts");
                }
                else if (owner_id == subject_id)
                {
                    MessageBox.Show("You can't add yourself to contacts");
                }
                else
                {
                    contactRepo.createContact(owner_id, subject_id, is_favourite);
                    MessageBox.Show("Contact added sucessfully");
                    ((WindowMain)this.Owner).loadContacts();
                }
            }
            else
            {
                MessageBox.Show("Select user to add");
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
