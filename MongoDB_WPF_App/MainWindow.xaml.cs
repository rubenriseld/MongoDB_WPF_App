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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDataAccess.DataAccess;

namespace MongoDB_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataAccess db = new DataAccess();
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            db.SetConnectionString(TxtBoxConnect.Text);
            if (await db.CheckConnection() == true)
            {
                TxtBoxStatus.Text = "Connection successful.";
                BtnConnect.IsEnabled = false;
                TxtBoxConnect.IsEnabled = false;
                await Task.Delay(1000);
                GridLogin.Visibility = Visibility.Hidden;
                await Task.Delay(150);

            }
            else TxtBoxStatus.Text = "Connection failed.";
        }
        private void TxtBoxConnect_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnConnect.IsEnabled = true;
        }
    }
}
