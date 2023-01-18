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
        private void TxtBoxConnect_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnConnect.IsEnabled = true;
        }

        private void BtnSelectCollection_Click(object sender, RoutedEventArgs e)
        {

            string selectedCollection = (CbxSelectCollection.SelectedItem as ComboBoxItem).Content.ToString();
            TxtBlockSelectedCollection.Text = "Selected collection: " + selectedCollection;

            StackPanelMutualButtons.Visibility = Visibility.Visible;

            if (selectedCollection == "artworks")
            {
                StackPanelCustomerButtons.Visibility = Visibility.Hidden;
                StackPanelArtworkButtons.Visibility = Visibility.Visible;
            }
            if (selectedCollection == "customers")
            {
                StackPanelArtworkButtons.Visibility = Visibility.Hidden;
                StackPanelCustomerButtons.Visibility = Visibility.Visible;
            }
            //try
            //{

            //    var selectedcollection = cbxselectcollection.selecteditem.tostring();
            //    if (selectedcollection != null)
            //    {
            //        db.selectedcollection = cbxselectcollection.selecteditem.tostring();
            //        var result = await db.getall();
            //        foreach (var item in result)
            //        {
            //            lbxresult.items.add($"{item.id}");
            //        }
            //    }
            //}
            //catch (exception ex)
            //{
            //    messagebox.show("error" + ex);
            //    return;
            //}

        }

        private void CbxSelectCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnSelectCollection.IsEnabled = true;
        }
    }
}
