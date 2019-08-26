using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VOiP_Communicator.Classes;
using VOiP_Communicator.Windows;

namespace VOiP_Communicator
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public delegate void UpdateButtonCallback(bool ButtonState);

        public static CallManager Manager;
        private List<Contact> resultsList;

        public void ButtonSetAsync(bool Connect, bool End, bool Mute)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() => Button_Connect.IsEnabled = Connect));
            Dispatcher.BeginInvoke(new ThreadStart(() => Button_End.IsEnabled = End));
            Dispatcher.BeginInvoke(new ThreadStart(() => Button_Mute.IsEnabled = Mute));
        }

        public void ButtonSet(bool Connect, bool End, bool Mute)
        {
            Button_Connect.IsEnabled = Connect;
            Button_End.IsEnabled = End;
            Button_Mute.IsEnabled = Mute;
        }
 
        public WindowMain()
        {
            InitializeComponent();

            loadAllContacts();
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
            refreshListBox();
            favourite.Content = "Mark as favourite";
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
           
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                string text = (string)listBox.SelectedItem;

                TextName.Text = text;
                Contact selectedItem = getContactByUsername(text);
                if (selectedItem.Photo != null)
                {
                    imageProfile.Source = PhotoHandler.ToImage(selectedItem.Photo);
                }
                if (selectedItem != null)
                {
                    if (selectedItem.IsFavourite)
                    {
                        favourite.Content = "Remove from favourites";
                    }
                    else favourite.Content = "Mark as favourite";
                }

                ButtonSet(true, false, false);
            }
            else
            {
                ButtonSet(false, false, false);
            }
        }

        public void loadContactsFromDb()
        {
            ContactsRepo contRepo = ContactsRepo.Instance();

            resultsList = contRepo.getAllCurrentContacts();

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            UserRepo userRepo = UserRepo.Instance();
            userRepo.setUserOffline(Globals.currentUserLogin);
        }

        private void loadAllContacts()
        {
            loadContactsFromDb();
            listBox.Items.Clear();
            foreach (Contact result in resultsList)
            {
                listBox.Items.Add(result.Username);
            }
        }

        private void loadOnlineContacts()
        {
            loadContactsFromDb();
            listBox.Items.Clear();

            foreach (Contact result in resultsList)
            {
                if((result.Status & 1) == 1)
                listBox.Items.Add(result.Username);
            }
        }

        private void loadFavouriteContacts()
        {
            loadContactsFromDb();
            listBox.Items.Clear();

            foreach (Contact result in resultsList)
            {
                if(result.IsFavourite)
                listBox.Items.Add(result.Username);
            }
        }

        private void FavouriteClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = listBox.SelectedItem;
            string text = (string)listBox.SelectedItem;

            ContactsRepo contRepo = ContactsRepo.Instance();
            Contact selectedContact = getContactByUsername(text);
            contRepo.toggleFavourite(selectedContact.SubjectId, Globals.currentUserId);
            MessageBox.Show("Change sucessfully saved");
            refreshListBox();
            favourite.Content = "Mark as favourite";
            TextName.Text = text;
            listBox.SelectedItem = selectedItem;
        }

        private Contact getContactByUsername(string Username)
        {
            foreach (Contact result in resultsList)
            {
                if (result.Username == Username)
                {
                    return result;
                }
            }

            return null;
        }

        public void refreshListBox()
        {
            if (comboBox.SelectedItem != null)
            {
                string text = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

                switch (text)
                {
                    case "All":

                        loadAllContacts();
                        break;
                    case "Online":

                        loadOnlineContacts();
                        break;
                    case "Favourite":

                        loadFavouriteContacts();
                        break;
                }
            }
            else
            {
                loadAllContacts();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Manager = new CallManager(this);
            ButtonSet(false, false, false);
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            string text = (string)listBox.SelectedItem;
            string ContactIP = getContactByUsername(text).Ip;
            int ContactID = getContactByUsername(text).SubjectId;
            Console.WriteLine(ContactIP);
            Manager.Call(ContactIP, ContactID);
            ButtonSet(false, true, true);
        }

        private void Button_End_Click(object sender, RoutedEventArgs e)
        {
            Manager.DropCall();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            string text = (string)listBox.SelectedItem;

            ContactsRepo contRepo = ContactsRepo.Instance();
            Contact selectedContact = getContactByUsername(text);
            contRepo.removeContact(selectedContact.SubjectId, Globals.currentUserId);
            MessageBox.Show("Change sucessfully saved");
            refreshListBox();
            ButtonSet(false, false, false);
        }

        private void Button_Options_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            OptionsWindow ow = new OptionsWindow();
            ow.Owner = this;
            ow.Show();
        }

        private void Button_History_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            CallHistoryWindow chw = new CallHistoryWindow();
            chw.Owner = this;
            chw.Show();
        }
    }
}
