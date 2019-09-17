﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace VOiP_Communicator.Windows
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private Image ProfileImage;

        public OptionsWindow()
        {
            InitializeComponent();
            var userRepo = new UserRepo();
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);

            profile_image.Source = PhotoHandler.ToImage(userRepo.fetchPhotoByUsername(Globals.currentUserId));
        }
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.Owner.IsEnabled = true;
        }
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            ProfileImage = new Image();
            //ProfileImage.Stretch = Stretch.Uniform;

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            if (op.ShowDialog() == true)
            {
                ProfileImage.Source = new BitmapImage(new Uri(op.FileName));
                profile_image.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try { 
            byte[] ImageData = getJPGFromImageControl(ProfileImage.Source as BitmapImage);
            var userRepo = new UserRepo();
            userRepo.savePhotoForUser(ImageData);
            }
            catch (Exception)
            {
                MessageBox.Show("No data loaded");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.Owner.IsEnabled = true;
        }

        private byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }
    }
}
