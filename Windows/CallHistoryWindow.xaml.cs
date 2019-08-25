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
using VOiP_Communicator.Classes;

namespace VOiP_Communicator
{
    /// <summary>
    /// Interaction logic for CallHistoryWindow.xaml
    /// </summary>
    public partial class CallHistoryWindow : Window
    {
        public CallHistoryWindow()
        {
            InitializeComponent();

        }

        private void ListView_CallHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void WindowCallHistory_Loaded(object sender, RoutedEventArgs e)
        {
            var resultsList = CallRepo.GetCallEntries(Globals.currentUserId);
            Console.WriteLine(resultsList.Count);
            foreach (var result in resultsList)
            {
                ListView_CallHistory.Items.Add(result);
            }

            
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.Owner.IsEnabled = true;
        }

        private void Button_Call_Click(object sender, RoutedEventArgs e)
        {
            var entry = (CallEntry)ListView_CallHistory.SelectedItem;
            var userRepo = new UserRepo();
            WindowMain.Manager.Call(userRepo.getColumnByIds(entry.User_ID, "ip_address"));

            this.Close();
            this.Owner.IsEnabled = true;
            
        }
    }
}
