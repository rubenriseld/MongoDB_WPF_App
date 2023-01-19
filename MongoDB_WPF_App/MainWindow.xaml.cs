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
            GridMain.Visibility = Visibility.Hidden;
            GridLogin.Visibility = Visibility.Visible;
        }

        //******************
        //    CONNECTION
        //******************
        private void TxtBoxConnect_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnConnect.IsEnabled = true;
        }
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            db.SetConnectionString(TxtBoxConnect.Text);
            if (await db.CheckConnection() == true)
            {
                TxtBlockStatus.Text = "Connection successful.";
                BtnConnect.IsEnabled = false;
                TxtBoxConnect.IsEnabled = false;
                await Task.Delay(1000);
                GridLogin.Visibility = Visibility.Hidden;
                await Task.Delay(150);
                GridMain.Visibility = Visibility.Visible;
            }
            else TxtBlockStatus.Text = "Connection failed.";
        }

        //******************
        //   CRUD BUTTONS
        //******************
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            ShowInputField();
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ShowInputField();
        }

        private void BtnCancelInput_Click(object sender, RoutedEventArgs e)
        {
            StackPanelCustomerInput.Visibility = Visibility.Hidden;
            StackPanelArtworkInput.Visibility = Visibility.Hidden;
            WrapPanelInputButtons.Visibility = Visibility.Hidden;
        }


        //******************
        //   SELECTION
        //******************
        private void CbxSelectCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnSelectCollection.IsEnabled = true;
        }
        private void BtnSelectCollection_Click(object sender, RoutedEventArgs e)
        {
            string selectedCollection = GetSelectedCollection();
            TxtBlockSelectedCollection.Text = "Selected collection: " + selectedCollection;

            StackPanelCustomerInput.Visibility = Visibility.Hidden;
            StackPanelArtworkInput.Visibility = Visibility.Hidden;
            WrapPanelInputButtons.Visibility = Visibility.Hidden;

            StackPanelMutualButtons.Visibility = Visibility.Visible;
        }
        private string GetSelectedCollection()
        {
            string selectedCollection = (CbxSelectCollection.SelectedItem as ComboBoxItem).Content.ToString();
            return selectedCollection;
        }
        private void ShowInputField()
        {
            string selectedCollection = GetSelectedCollection();
            if (selectedCollection == "artworks")
            {
                StackPanelCustomerInput.Visibility = Visibility.Hidden;
                StackPanelArtworkInput.Visibility = Visibility.Visible;
            }
            if (selectedCollection == "customers")
            {
                StackPanelArtworkInput.Visibility = Visibility.Hidden;
                StackPanelCustomerInput.Visibility = Visibility.Visible;
            }
            WrapPanelInputButtons.Visibility = Visibility.Visible;
        }

        
    }
}
