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
using MongoDataAccess.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataAccess db = new DataAccess();
        ArtworkModel temp = new ArtworkModel();
        public MainWindow()
        {
            InitializeComponent();
            GridMain.Visibility = Visibility.Hidden;
            GridLogin.Visibility = Visibility.Visible;
        }

        //******************
        //    CONNECTION
        //******************

        //enable connect button when text is entered in textbox
        private void TxtBoxConnect_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnConnect.IsEnabled = true;
        }
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            //check connection with database
            db.SetConnectionString(TxtBoxConnect.Text);
            if (await db.CheckConnection() == true)
            {
                TxtBlockStatus.Text = "Connection successful.";
                BtnConnect.IsEnabled = false;
                TxtBoxConnect.IsEnabled = false;
                await Task.Delay(800);
                GridLogin.Visibility = Visibility.Hidden;
                await Task.Delay(150);
                GridMain.Visibility = Visibility.Visible;
            }
            else TxtBlockStatus.Text = "Connection failed.";
        }

        //******************************************************
        //
        //                  CRUD BUTTONS
        //
        //******************************************************

        //******************
        //    READ ALL
        //******************

        //fill result view with all objects from collection
        private async void BtnReadAll_Click(object sender, RoutedEventArgs e)
        {
            HideInputField();
            LbxResult.Items.Clear();
            string selectedCollection = GetSelectedCollection();
            if (selectedCollection == "artworks")
            {
                var result = await db.GetAllArtworks();
                if (result.Count == 0) MessageBox.Show("No artworks in collection.");
                foreach (var item in result)
                {
                    AddArtworkToResultView(item);
                }
            }
            if (selectedCollection == "customers")
            {
                var result = await db.GetAllCustomers();
                if (result.Count == 0) MessageBox.Show("No customers in collection.");
                foreach (var item in result)
                {
                    AddCustomerToResultView(item);
                }
            }
        }

        //******************
        //  READ BY INDEX
        //******************

        //show search input field
        private void BtnReadByIndex_Click(object sender, RoutedEventArgs e)
        {
            HideInputField();
            StackPanelIndexSearch.Visibility = Visibility.Visible;
            WrapPanelSearchButtons.Visibility = Visibility.Visible;
        }
        //search selected collection for matching index nr
        private async void BtnEnterSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedCollection = GetSelectedCollection();
                int index = Convert.ToInt32(TxtBoxIndexSearch.Text);
                LbxResult.Items.Clear();
                if (selectedCollection == "artworks")
                {
                    var result = await db.GetArtworkByIndex(index);
                    if (result == null)
                    {
                        MessageBox.Show("Could not find an artwork with index: " + index);
                    }
                    else AddArtworkToResultView(result);
                    ClearInputField();
                }
                if (selectedCollection == "customers")
                {
                    var result = await db.GetCustomerByIndex(index);
                    if (result == null)
                    {
                        MessageBox.Show("Could not find a customer with index: " + index);
                    }
                    else AddCustomerToResultView(result);
                    ClearInputField();
                }
            }
            catch
            {
                MessageBox.Show("Invalid input, please enter a number and try again.");
                TxtBoxIndexSearch.Text = "";
                return;
            }
        }
        //hide search input field
        private void BtnCancelSearch_Click(object sender, RoutedEventArgs e)
        {
            HideInputField();
            ClearInputField();
        }

        //******************
        //     CREATE
        //******************

        //show create input field
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            ClearInputField();
            HideInputField();
            ShowInputField();
            WrapPanelCreateButtons.Visibility = Visibility.Visible;
        }

        //create new object from input field values
        private async void BtnEnterCreateInput_Click(object sender, RoutedEventArgs e)
        {
            string selectedCollection = GetSelectedCollection();
            if (selectedCollection == "artworks")
            {
                if (ValidateArtworkInput() == true)
                {
                    var artwork = CreateArtworkModelFromInput();
                    var existingArtworks = await db.GetAllArtworks();

                    //auto-incrementing index
                    if (existingArtworks.Count() == 0)
                    {
                        artwork.Index = 1;
                    }
                    else
                    {
                        var sortedArtworks = existingArtworks.OrderByDescending(a => a.Index).ToList();
                        var highestIndex = sortedArtworks.First();

                        artwork.Index = highestIndex.Index + 1;
                    }
                    await db.CreateArtwork(artwork);
                    MessageBox.Show("Artwork added successfully!");
                    ClearInputField();
                }
                else return;
            }
            if (selectedCollection == "customers")
            {
                if (ValidateCustomerInput() == true)
                {
                    var customer = CreateCustomerModelFromInput();
                    var existingCustomers = await db.GetAllCustomers();

                    //auto-incrementing index
                    if (existingCustomers.Count() == 0)
                    {
                        customer.Index = 1;
                    }
                    else
                    {
                        var sortedCustomers = existingCustomers.OrderByDescending(c => c.Index).ToList();
                        var highestIndex = sortedCustomers.First();

                        customer.Index = highestIndex.Index + 1;
                    }
                    await db.CreateCustomer(customer);
                    MessageBox.Show("Customer added successfully!");
                    ClearInputField();
                }
                else return;
            }
        }

        //hide create input fields
        private void BtnCancelCreateInput_Click(object sender, RoutedEventArgs e)
        {
            HideInputField();
            ClearInputField();
        }

        //******************
        //     UPDATE
        //******************        

        //update button by result view, moves selected object to input fields for editing
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ClearInputField();
            HideInputField();
            ShowInputField();

            WrapPanelUpdateButtons.Visibility = Visibility.Visible;

            string selectedCollection = GetSelectedCollection();
            if (selectedCollection == "artworks")
            {
                temp = GetArtworkFromSelection();

                TxtBoxArtworkTitle.Text = temp.Title;
                TxtBoxArtworkDescription.Text = temp.Description;
                TxtBoxArtworkPrice.Text = temp.Price.ToString();
                if (temp.Sold.ToString() == "False") CbxArtworkSold.SelectedIndex = 0;
                else
                {
                    CbxArtworkSold.SelectedIndex = 1;
                    if (temp.SoldTo != null) TxtBoxArtworkSoldTo.Text = temp.SoldTo.Id.ToString();
                }
            }
            if (selectedCollection == "customers")
            {
                var oldCustomer = GetCustomerFromSelection();

                TxtBoxCustomerFirstName.Text = oldCustomer.FirstName;
                TxtBoxCustomerLastName.Text = oldCustomer.LastName;
                TxtBoxCustomerPhoneNumber.Text = oldCustomer.PhoneNumber;
                TxtBoxCustomerEmailAddress.Text = oldCustomer.EmailAddress;
            }
        }

        //update the selected object with changes made in input fields
        private async void BtnEnterUpdateInput_Click(object sender, RoutedEventArgs e)
        {
            string selectedCollection = GetSelectedCollection();
            if (selectedCollection == "artworks")
            {
                if (ValidateArtworkInput() == true)
                {
                    var artwork = temp;
                    var artworkChanges = CreateArtworkModelFromInput();
                    artwork.Title = artworkChanges.Title;
                    artwork.Description = artworkChanges.Description;
                    artwork.Price = artworkChanges.Price;
                    artwork.Sold = artworkChanges.Sold;

                    //assign customer to artwork if selected
                    CustomerModel soldTo = (CustomerModel)(((FrameworkElement)(TxtBoxArtworkSoldTo)).DataContext);
                    artwork.SoldTo = soldTo;

                    //remove assigned customer if Sold bool is changed to false
                    string soldSelection = (CbxArtworkSold.SelectedItem as ComboBoxItem).Content.ToString();
                    if (soldSelection == "false")
                    {
                        artwork.SoldTo = null;
                    }

                    await db.UpdateArtwork(artwork);

                    MessageBox.Show("Artwork updated successfully!");
                    ClearInputField();
                    HideInputField();
                }
                else return;
            }
            if (selectedCollection == "customers")
            {
                if (ValidateArtworkInput() == true)
                {
                    var customer = GetCustomerFromSelection();
                    var customerChanges = CreateCustomerModelFromInput();
                    customer.FirstName = customerChanges.FirstName;
                    customer.LastName = customerChanges.LastName;
                    customer.PhoneNumber = customerChanges.PhoneNumber;
                    customer.EmailAddress = customerChanges.EmailAddress;

                    await db.UpdateCustomer(customer);

                    //update customer reference in artworks
                    var artworkList = await db.GetAllArtworks();
                    if (artworkList.Count > 0)
                    {
                        foreach (var item in artworkList)
                        {
                            if (item.SoldTo != null && item.SoldTo.Id == customer.Id)
                            {
                                item.SoldTo.FirstName = customer.FirstName;
                                item.SoldTo.LastName = customer.LastName;
                                item.SoldTo.PhoneNumber = customer.PhoneNumber;
                                item.SoldTo.EmailAddress = customer.EmailAddress;
                                await db.UpdateArtwork(item);
                            }
                        }
                    }
                    MessageBox.Show("Customer updated successfully!");
                    ClearInputField();
                    HideInputField();
                }
                else return;
            }
        }

        //hide update input fields
        private void BtnCancelUpdateInput_Click(object sender, RoutedEventArgs e)
        {
            HideInputField();
            ClearInputField();
        }

        //******************
        //     DELETE
        //******************

        //delete selected object from result view
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string selectedCollection = GetSelectedCollection();
            if (selectedCollection == "artworks")
            {
                var artwork = GetArtworkFromSelection();
                db.DeleteArtwork(artwork);
                LbxResult.Items.Remove(LbxResult.SelectedItem);
                MessageBox.Show("Artwork deleted successfully!");
            }
            if (selectedCollection == "customers")
            {
                var customer = GetCustomerFromSelection();
                db.DeleteCustomer(customer);
                LbxResult.Items.Remove(LbxResult.SelectedItem);
                MessageBox.Show("Customer deleted successfully!");
            }
        }

        //******************************************************
        //
        //                  MISC OPERATIONS
        //
        //******************************************************

        //******************
        //   SELECTION
        //******************

        //enable select button to set current collection when an option is chosen
        private void CbxSelectCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnSelectCollection.IsEnabled = true;
        }
        //set current collection
        private void BtnSelectCollection_Click(object sender, RoutedEventArgs e)
        {
            string selectedCollection = GetSelectedCollection();
            TxtBlockSelectedCollection.Text = "Selected collection: " + selectedCollection;
            HideInputField();
            LbxResult.Items.Clear();
            StackPanelReadCreateButtons.Visibility = Visibility.Visible;
        }
        //get current collection for operations
        private string GetSelectedCollection()
        {
            string selectedCollection = (CbxSelectCollection.SelectedItem as ComboBoxItem).Content.ToString();
            return selectedCollection;
        }

        //******************
        //   INPUT FIELDS
        //******************
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
        }
        private void HideInputField()
        {
            StackPanelCustomerInput.Visibility = Visibility.Hidden;
            StackPanelArtworkInput.Visibility = Visibility.Hidden;
            StackPanelIndexSearch.Visibility= Visibility.Hidden;
            WrapPanelCreateButtons.Visibility = Visibility.Hidden;
            WrapPanelUpdateButtons.Visibility = Visibility.Hidden;
            WrapPanelSearchButtons.Visibility = Visibility.Hidden;
        }
        private void ClearInputField()
        {
            TxtBoxArtworkTitle.Text = "";
            TxtBoxArtworkDescription.Text = "";
            TxtBoxArtworkPrice.Text = "";
            CbxArtworkSold.SelectedIndex = 0;
            TxtBoxArtworkSoldTo.Text = "";

            TxtBoxCustomerFirstName.Text = "";
            TxtBoxCustomerLastName.Text = "";
            TxtBoxCustomerPhoneNumber.Text = "";
            TxtBoxCustomerEmailAddress.Text = "";

            TxtBoxIndexSearch.Text = "";
        }

        //shows SoldTo field only when sold==true, and fill result view with customers for selection on create/update with ArtworkModel
        private async void CbxArtworkSold_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string soldSelection = (CbxArtworkSold.SelectedItem as ComboBoxItem).Content.ToString();
            if (soldSelection == "true")
            {
                StackPanelArtworkCustomer.Visibility = Visibility.Visible;
                LbxResult.Items.Clear();
                var result = await db.GetAllCustomers();
                if (result.Count == 0) MessageBox.Show("No customers in collection.");
                foreach (var item in result)
                {
                    AddCustomerToResultView(item);
                }
                BtnSelect.IsEnabled = true;
            }
            else
            {
                StackPanelArtworkCustomer.Visibility = Visibility.Hidden;
            }
        }

        //show update/delete buttons when an object in result view is selected
        private void LbxResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnUpdate.IsEnabled = true;
            BtnDelete.IsEnabled = true;
            if (LbxResult.SelectedItem == null)
            {
                BtnUpdate.IsEnabled = false;
                BtnDelete.IsEnabled = false;
            }
        }

        //******************
        //   RESULT VIEW
        //******************
        
        //adding objects to result view (Listbox)
        private void AddArtworkToResultView(ArtworkModel artwork)
        {
            ListBoxItem addedResult = new ListBoxItem();
            addedResult.Content = $"Object ID: {artwork.Id}\n" +
                                    $"Index: {artwork.Index}\n" +
                                    $"Title: {artwork.Title}\n" +
                                    $"Description: {artwork.Description}\n" +
                                    $"Price: {artwork.Price}\n" +
                                    $"Sold: {artwork.Sold}";
            if (artwork.Sold == true) addedResult.Content += $"\nSold to: {artwork.SoldTo.FullName}";
            addedResult.DataContext = artwork;

            LbxResult.Items.Add(addedResult);

            StackPanelUpdateDeleteButtons.Visibility = Visibility.Visible;
        }
        private void AddCustomerToResultView(CustomerModel customer)
        {
            ListBoxItem addedResult = new ListBoxItem();
            addedResult.Content = $"Object ID: {customer.Id}\n" +
                                    $"Index: {customer.Index}\n" +
                                    $"Name: {customer.FullName}\n" +
                                    $"Phone number: {customer.PhoneNumber}\n" +
                                    $"Email address: {customer.EmailAddress}";
            addedResult.DataContext = customer;
            LbxResult.Items.Add(addedResult);
            StackPanelUpdateDeleteButtons.Visibility = Visibility.Visible;
        }

        //get models from result view
        private ArtworkModel GetArtworkFromSelection()
        {
            ArtworkModel artwork = (ArtworkModel)(((FrameworkElement)(LbxResult.SelectedItem)).DataContext);
            return artwork;
        }
        private CustomerModel GetCustomerFromSelection()
        {
            CustomerModel customer = (CustomerModel)(((FrameworkElement)(LbxResult.SelectedItem)).DataContext);
            return customer;
        }

        //clear result view
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            LbxResult.Items.Clear();
        }

        //******************
        // MODEL CREATION
        //******************

        //creation from input fields
        private ArtworkModel CreateArtworkModelFromInput()
        {
            if (ValidateArtworkInput() == true)
            {
                try
                {
                    string title = TxtBoxArtworkTitle.Text;
                    string description = TxtBoxArtworkDescription.Text;
                    double price = Convert.ToDouble(TxtBoxArtworkPrice.Text);
                    bool sold = Convert.ToBoolean((CbxArtworkSold.SelectedItem as ComboBoxItem).Content.ToString());
                    CustomerModel soldTo = (CustomerModel)(((FrameworkElement)(TxtBoxArtworkSoldTo)).DataContext);

                    ArtworkModel artwork = new ArtworkModel()
                    {
                        Title = title,
                        Description = description,
                        Price = price,
                        Sold = sold,
                        SoldTo = soldTo
                    };
                    return artwork;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex + "\n An error occured, please try again.");
                    return null;
                }
            }
            else return null;
        }
        private CustomerModel CreateCustomerModelFromInput()
        {
            try
            {
                string firstName = TxtBoxCustomerFirstName.Text;
                string lastName = TxtBoxCustomerLastName.Text;
                string phoneNumber = TxtBoxCustomerPhoneNumber.Text;
                string emailAddress = TxtBoxCustomerEmailAddress.Text;

                CustomerModel customer = new CustomerModel()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    EmailAddress = emailAddress
                };
                return customer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + "\n An error occured, please try again.");
                return null;
            }
        }

        //validation
        private bool ValidateArtworkInput()
        {
            if (TxtBoxArtworkTitle.Text == "" || TxtBoxArtworkTitle.Text == null)
            {
                MessageBox.Show("Please input artwork title.");
                return false;
            }
            if (TxtBoxArtworkDescription.Text == "" || TxtBoxArtworkDescription.Text == null)
            {
                MessageBox.Show("Please input artwork description.");
                return false;
            }
            if (TxtBoxArtworkPrice.Text == "" || TxtBoxArtworkPrice.Text == null)
            {
                MessageBox.Show("Please input artwork price.");
                return false;
            }
            if (CbxArtworkSold.SelectedIndex == -1)
            {
                MessageBox.Show("Please select sale status.");
                return false;
            }
            return true;
        }
        private bool ValidateCustomerInput()
        {
            if (TxtBoxCustomerFirstName.Text == "" || TxtBoxCustomerFirstName.Text == null)
            {
                MessageBox.Show("Please input the customers first name.");
                return false;
            }
            if (TxtBoxCustomerLastName.Text == "" || TxtBoxCustomerLastName.Text == null)
            {
                MessageBox.Show("Please input the customers last name.");
                return false;
            }
            if (TxtBoxCustomerPhoneNumber.Text == "" || TxtBoxCustomerPhoneNumber.Text == null)
            {
                MessageBox.Show("Please input customer phone number.");
                return false;
            }
            if (TxtBoxCustomerEmailAddress.Text == "" || TxtBoxCustomerEmailAddress.Text == null)
            {
                MessageBox.Show("Please input customer email address.");
                return false;
            }
            return true;
        }

        //select customer from result view to assign as SoldTo for an ArtworkModel
        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            var customer = GetCustomerFromSelection();
            TxtBoxArtworkSoldTo.DataContext = customer;
            TxtBoxArtworkSoldTo.Text = customer.Id.ToString();
        }        
    }
}