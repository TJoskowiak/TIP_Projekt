﻿using System;
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
using VOiP_Communicator.Classes;

namespace VOiP_Communicator
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        private List<Contact> resultsList;
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
            refreshListBox();
            favourite.Content = "Mark as favourite";
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = (string)listBox.SelectedItem;

            textBlock.Text = text;
            Contact selectedItem = getContactByUsername(text);
            if (selectedItem != null)
            {
                if (selectedItem.IsFavourite)
                {
                    favourite.Content = "Remove from favourites";
                }
                else favourite.Content = "Mark as favourite";
            }
        }

        public void loadContacts()
        {
            listBox.Items.Clear();
            loadContactsFromDb();

            foreach (Contact result in resultsList)
            {
                listBox.Items.Add(result.Username);
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
            textBlock.Text = text;
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

        private void refreshListBox()
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
    }
}
