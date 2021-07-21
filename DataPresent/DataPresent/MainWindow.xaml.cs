using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
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
using System.Globalization;

namespace DataPresent
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool startedConnNorthwind = false;       //to make sure there is a connection to close/not another connection running
        public bool updateActive = false;               //to make sure you can't accidentaly change/enter anything! 
        public bool insertActive = false;               //-||-
        public bool deleteActive = false;               //-||-
        public bool formLocked = true;                  //to lock the form except for when user needs to edit it. false locks ID
        public bool tabLocked = true;
        public bool filterActive = false;
        public string currentFilter = "none";
        DataTable tableProducts = new DataTable();
        DataTable tableCompanies = new DataTable();
        DataTable tableFilter = new DataTable();
        Random randInt = new Random();                  //if random is needed
        

        public int currentIDProduct = 0;                  //starts from 0!
        public int currentIDCompany = 0;
        public int maxIDProduct = 0;                      //important for insert
        public int maxIDCompany = 0;

        public List<Company> companyList = new List<Company>();     //Lists for Comboboxes
        public List<Category> categoryList = new List<Category>();  //Lists for Comboboxes
        Northwind northwind = new Northwind();                      //SQLClass, not too universal or specific. 


        public MainWindow()
        {
            InitializeComponent();

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();                               //Exit the Programm
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (startedConnNorthwind == false)
            {
                //build the connection to Northwind database
                MessageBox.Show("Connection established", Title = "Connectionwindow");
                northwind.startConnection();
                startedConnNorthwind = true;    

                //Enable/Disable Buttons
                //general
                btnConnect.IsEnabled = false;
                btnDisconnect.IsEnabled = true;
                //Product
                btnInsert.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnProductFilterActivate.IsEnabled = true;
                //Company
                btnAddCompany.IsEnabled = true;
                btnEditCompany.IsEnabled = true;
                btnDeleteCompany.IsEnabled = true;

                //Fill tables with values
                tableProducts = northwind.getSqlTableUniversal("select * from vw_simpleProductOverview", tableProducts);
                dgvProducts.ItemsSource = tableProducts.DefaultView;

                tableCompanies = northwind.getSqlTableUniversal("Select * from vw_simpleCompanyOverview", tableCompanies);
                dgvCompanies.ItemsSource = tableCompanies.DefaultView;

                //Insert values into Groupboxes
                companyList = northwind.getCompanySqlList("Select * from vw_simpleCompanyOverview", companyList);
                cboCompany.ItemsSource = companyList;           //Bind Combobox to List
                cboFilterCompany.ItemsSource = companyList;
                
                categoryList = northwind.getCategorySqlList("Select CategoryName from Categories", categoryList);
                cboCategory.ItemsSource = categoryList;         //Bind Combobox to List
                cboFilterCategory.ItemsSource = categoryList;

                //Enable/Disable Tab, Datagrids and Groupboxes
                //Tab
                tabLocked = false;
                tabNorthwind.IsEnabled = true;
                //Datagrid
                dgvProducts.IsEnabled = true;
                txtIDProduct.IsReadOnly = true;
                dgvCompanies.IsEnabled = true;
                txtIDCompany.IsReadOnly = true;
                
                //Groupbox/lock IDs
                grbProductDataRow.Visibility = Visibility.Visible;
                grbCompanyDataRow.Visibility = Visibility.Hidden;
                grbProductFilter.Visibility = Visibility.Visible;
                btnProductFilterActivate.Visibility = Visibility.Visible;
                txtIDProduct.IsReadOnly = false;
                txtIDCompany.IsReadOnly = false;

                //get Max IDs for each table
                maxIDProduct = northwind.getMaxIDUniversal(tableProducts);
                maxIDCompany = northwind.getMaxIDUniversal(tableCompanies);

                //set CurrentID for each table
                currentIDProduct = 0;
                currentIDCompany = 0;

                //Preselect first entry
                dgvProducts.SelectedItem = dgvProducts.Items[currentIDProduct];
                dgvCompanies.SelectedItem = dgvCompanies.Items[currentIDCompany];
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (startedConnNorthwind == true)
            {
                //end connection to Database
                MessageBox.Show("Disconnected from Server", Title = "Connectionwindow");
                northwind.endConenction();
                startedConnNorthwind = false;

                //Clear Items
                {
                    //Productform
                    txtIDProduct.Clear();
                    txtProductName.Clear();
                    cboCompany.ItemsSource = null;
                    cboCategory.ItemsSource = null;
                    txtQuantityPerUnit.Clear();
                    txtUnitPrice.Clear();
                    txtUnitsInStock.Clear();
                    chkDiscontinued.IsChecked = false;

                    //Companyform
                    txtIDProduct.Clear();
                    txtCompanyName.Clear();
                    txtContactName.Clear();
                    txtTitle.Clear();
                    txtAddress.Clear();
                    txtCity.Clear();
                    txtPostalCode.Clear();
                    txtPhone.Clear();

                    //Tables
                    tableProducts.Clear();
                    dgvProducts.ItemsSource = null;
                    tableCompanies.Clear();
                    dgvCompanies.ItemsSource = null;
                }

                //Enable/Disable Buttons
                //general
                btnConnect.IsEnabled = true;
                btnDisconnect.IsEnabled = false;
                //product
                btnInsert.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnProductFilterActivate.IsEnabled = false;
                //Company
                btnAddCompany.IsEnabled = false;
                btnEditCompany.IsEnabled = false;
                btnDeleteCompany.IsEnabled = false;

                //Hide/Disable groupboxes
                tabNorthwind.IsEnabled = false;
                grbProductDataRow.Visibility = Visibility.Hidden;
                grbCompanyDataRow.Visibility = Visibility.Hidden;
                grbProductFilter.Visibility = Visibility.Hidden;
                btnProductFilterActivate.Visibility = Visibility.Hidden;

            }
        }

        private void pngJelly_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Little logic helper for errors which don't let the Program crash
            MessageBox.Show("Current Variables at the moment: " +
                    "\n-startedConnNorthwind: " + startedConnNorthwind +
                    "\n-updateActive: " + updateActive +
                    "\n-insertActive: " + insertActive +
                    "\n-deleteActive: " + deleteActive +
                    "\n-formLocked: " + formLocked +
                    "\n-tabLocked: " + tabLocked +
                    "\n-filterActive: " + filterActive +
                    "\n-currentFilter: " + currentFilter + 
                    "\n-currentIDProduct: " + currentIDProduct +
                    "\n-maxIDProduct: " + maxIDProduct +
                    "\n-currentIDCompany" + currentIDCompany +
                    "\n-maxIDCompany: " + maxIDCompany, Title = "Jelly Menu");
        }

        private void dgvNorthwind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formLocked == true)
            {
                currentIDProduct = dgvProducts.SelectedIndex;      //save new item 
            }
            else
            {
                object item = dgvProducts.Items[currentIDProduct]; //stay with current item
                dgvProducts.SelectedItem = item;
            }
        }

        private void dgvCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formLocked == true)
            {
                currentIDCompany = dgvCompanies.SelectedIndex;      //save new item 
            }
            else
            {
                object item = dgvCompanies.Items[currentIDCompany]; //stay with current item
                dgvCompanies.SelectedItem = item;
            }
        }

        private void cboCompany_DropDownOpened(object sender, EventArgs e)
        {
            //lock dropdown
            if (formLocked == true)
            {
                cboCompany.IsDropDownOpen = false;
            }
            else
            {
                cboCompany.IsDropDownOpen = true;
            }
        }

        private void cboCategory_DropDownOpened(object sender, EventArgs e)
        {
            //lock dropdown
            if (formLocked == true)
            {
                cboCategory.IsDropDownOpen = false;
            }
            else
            {
                cboCategory.IsDropDownOpen = true;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            
            if (updateActive == false)
            {
                {
                    //open Form for datachange
                    formLocked = false;
                    txtProductName.IsReadOnly = false;
                    txtQuantityPerUnit.IsReadOnly = false;
                    txtUnitPrice.IsReadOnly = false;
                    txtUnitsInStock.IsReadOnly = false;
                    chkDiscontinued.IsEnabled = true;
                }
                //disable/change Buttons
                btnDelete.IsEnabled = false;
                btnInsert.IsEnabled = false;
                btnDisconnect.IsEnabled = false;
                btnProductFilterActivate.IsEnabled = false;
                btnUpdate.Content = "Save Changes";
                
                //to get into update/Insert
                updateActive = true;
                    
                //to make sure that User can't change stuff through datagrid
                dgvProducts.IsEnabled = false;
                tabLocked = true;
                tabNorthwind.IsEnabled = false;

            }
            else
            {
                {
                    //lock form again
                    formLocked = true;
                    txtProductName.IsReadOnly = true;
                    txtQuantityPerUnit.IsReadOnly = true;
                    txtUnitPrice.IsReadOnly = true;
                    txtUnitsInStock.IsReadOnly = true;
                    chkDiscontinued.IsEnabled = false;
                }
                //Enable/Change Buttons 
                btnDelete.IsEnabled = true;
                btnInsert.IsEnabled = true;
                btnDisconnect.IsEnabled = true;
                btnProductFilterActivate.IsEnabled = true;
                btnUpdate.Content = "Edit Product";
                
                //to get into fillpath
                updateActive = false;

                //to make datagrid available again
                dgvProducts.IsEnabled = true;
                tabLocked = false;
                tabNorthwind.IsEnabled = true;

                //--Update 
                //Fill Product with values to give to SQLClass
                Product product = new Product(
                    int.Parse(txtIDProduct.Text),
                    txtProductName.Text.ToString(),
                    cboCompany.Text.ToString(),
                    cboCategory.Text.ToString(),
                    txtQuantityPerUnit.Text.ToString(),
                    Decimal.Parse(txtUnitPrice.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                    int.Parse(txtUnitsInStock.Text),
                    chkDiscontinued.IsChecked.Value
                    );

                //give SQLClass Commandstring and Product for update
                northwind.updateSqlProduct("update vw_simpleProductOverview " +
                    "set ProductName = @ProductName, " +
                    "CompanyName = @CompanyName, " +
                    "CategoryName = @CategoryName, " +
                    "QuantityPerUnit = @QuantityPerUnit, " +
                    "UnitPrice = @UnitPrice, " +
                    "UnitsInStock = @UnitsInStock, " +
                    "Discontinued = @Discontinued " +
                    "where ProductID = " + product.ID + ";", product);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            
            if (insertActive == false)
            {
                //insert vorbereiten
                
                //lock tab and datagrid
                dgvProducts.IsEnabled = false;
                tabLocked = true;
                tabNorthwind.IsEnabled = true;
                dgvProducts.UnselectAll();

                //Hide ID & prepare for new ID
                txtIDProduct.IsReadOnly = true;
                txtIDProduct.Visibility = Visibility.Hidden;
                lblFixProductID.Visibility = Visibility.Hidden;
                txtIDProduct.Text = maxIDProduct.ToString();
                txtIDProduct.IsEnabled = false;

                //unlock form
                formLocked = false;
                txtProductName.IsReadOnly = false;
                txtQuantityPerUnit.IsReadOnly = false;
                txtUnitPrice.IsReadOnly = false;
                txtUnitsInStock.IsReadOnly = false;
                chkDiscontinued.IsEnabled = true;
                chkDiscontinued.IsChecked = false;

                //disable/change Buttons
                btnDelete.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                btnDisconnect.IsEnabled = false;
                btnProductFilterActivate.IsEnabled = false;
                btnInsert.Content = "Create Product";

                //clear form
                txtProductName.Text = null;
                txtUnitPrice.Text = null;
                txtUnitsInStock.Text = null;
                txtQuantityPerUnit.Text = null;
                        
                //get to InsertPath
                insertActive = true;

            }
            else
            {
                {
                    //lock form
                    formLocked = true;
                    txtIDProduct.IsReadOnly = true;
                    txtIDProduct.IsEnabled = true;
                    txtIDProduct.Visibility = Visibility.Visible;
                    lblFixProductID.Visibility = Visibility.Visible;
                    txtProductName.IsReadOnly = true;
                    txtQuantityPerUnit.IsReadOnly = true;
                    txtUnitPrice.IsReadOnly = true;
                    txtUnitsInStock.IsReadOnly = true;
                    chkDiscontinued.IsEnabled = false;
                }
                //unlock buttons
                btnDelete.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnDisconnect.IsEnabled = true;
                btnProductFilterActivate.IsEnabled = true;
                btnInsert.Content = "Add Product";
                
                //leave InsertPath
                insertActive = false;
                
                //Id textboxes are left empty
                if (string.IsNullOrEmpty(txtProductName.Text))
                {
                    txtProductName.Text = "Some kind of Name";
                }
                if (string.IsNullOrEmpty(txtQuantityPerUnit.Text))
                {
                    txtQuantityPerUnit.Text = "Some kind of Unit";
                }
                if (string.IsNullOrEmpty(txtUnitPrice.Text))
                {
                    txtUnitPrice.Text = "0";
                }
                if (string.IsNullOrEmpty(txtUnitsInStock.Text))
                {
                    txtUnitsInStock.Text = "0";
                }
                if (string.IsNullOrEmpty(cboCategory.Text))
                {
                    cboCategory.SelectedItem = randInt.Next(cboCategory.Items.Count)-1;
                }
                if (string.IsNullOrEmpty(cboCompany.Text))
                {
                    cboCategory.SelectedItem = randInt.Next(cboCompany.Items.Count)-1;
                }

                //fill Product with values
                Product product = new Product(
                    maxIDProduct,
                    txtProductName.Text,
                    cboCompany.Text,
                    cboCategory.Text,
                    txtQuantityPerUnit.Text,
                    Decimal.Parse(txtUnitPrice.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                    int.Parse(txtUnitsInStock.Text),
                    chkDiscontinued.IsChecked.Value
                    );


                //Insertcommand
                    tableProducts = northwind.insertSqlProduct("Insert into vw_simpleProductOverview" +
                "(ProductName, CompanyName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, Discontinued) " +
                "values(@ProductName, @CompanyName, @CategoryName, @QuantityPerUnit, @UnitPrice, @UnitsInStock, @Discontinued)", product, tableProducts);

                //enable datagrid and tab
                dgvProducts.IsEnabled = true;
                tabLocked = false;
                tabNorthwind.IsEnabled = true;
                

                //Set new MaxID
                maxIDProduct = northwind.getMaxIDUniversal(tableProducts);
                txtIDProduct.Text = maxIDProduct.ToString();

                //set selected item to maxID(new insertrow)
                dgvProducts.SelectedItem = maxIDProduct;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //Make sure Deletion is wanted, then delete entry
            if (MessageBox.Show("Willst du wirklich das Produkt \"" + txtProductName.Text + "\" mit der ID " + txtIDProduct.Text + " löschen ? ", Title = "Achtung!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DataRowView delRow = (DataRowView)dgvProducts.SelectedItem;
                tableProducts = northwind.deleteSqlRowUniversal("Delete from vw_simpleProductOverview where ProductID = @ID", int.Parse(txtIDProduct.Text), tableProducts, delRow);
            }
        }

        private void tabNorthwind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabProduct.IsSelected)
            {
                //When user picks Producttab
                if (startedConnNorthwind == true)
                {
                    //Show right form
                    grbCompanyDataRow.Visibility = Visibility.Hidden;
                    grbProductDataRow.Visibility = Visibility.Visible;
                    btnProductFilterActivate.Visibility = Visibility.Visible;

                    //Show right datagrid
                    dgvProducts.IsEnabled = true;
                    dgvCompanies.IsEnabled = false;
                    dgvProducts.SelectedItem = currentIDProduct;

                    grbProductFilter.Visibility = Visibility.Visible;
                }
            }
            if (TabCompany.IsSelected)
            {
                //When user picks Producttab
                if (startedConnNorthwind == true)
                {
                    //Show right form
                    grbCompanyDataRow.Visibility = Visibility.Visible;
                    grbProductDataRow.Visibility = Visibility.Hidden;
                    btnProductFilterActivate.Visibility = Visibility.Hidden;

                    //Show right datagrid
                    dgvCompanies.IsEnabled = true;
                    dgvProducts.IsEnabled = false;
                    dgvCompanies.SelectedItem = currentIDCompany;

                    //Show right filters
                    grbProductFilter.Visibility = Visibility.Hidden;
                }
            }
            //if(TabEmployee.IsSelected)
            
        }

        private void btnEditCompany_Click(object sender, RoutedEventArgs e)
        {
            if (updateActive == false)
            {
                {
                    //open Form for Datachange
                    formLocked = false;
                    txtCompanyName.IsReadOnly = false;
                    txtContactName.IsReadOnly = false;
                    txtTitle.IsReadOnly = false;
                    txtAddress.IsReadOnly = false;
                    txtCity.IsReadOnly = false;
                    txtPostalCode.IsReadOnly = false;
                    txtPhone.IsReadOnly = false;
                    txtCountry.IsReadOnly = false;
                }
                //disable/change Buttons
                btnAddCompany.IsEnabled = false;
                btnDeleteCompany.IsEnabled = false;
                btnEditCompany.Content = "Save Product";
                btnDisconnect.IsEnabled = false;

                //to get to update
                updateActive = true;

                //to make sure that User can't change stuff through datagrid
                dgvCompanies.IsEnabled = false;
                tabLocked = true;
                tabNorthwind.IsEnabled = false;
            }
            else
            {
                {
                    //lock Form again
                    formLocked = true;
                    txtCompanyName.IsReadOnly = true;
                    txtContactName.IsReadOnly = true;
                    txtTitle.IsReadOnly = true;
                    txtAddress.IsReadOnly = true;
                    txtCity.IsReadOnly = true;
                    txtPostalCode.IsReadOnly = true;
                    txtPhone.IsReadOnly = true;
                    txtCountry.IsReadOnly = true;
                }
                //enable/change Buttons
                btnAddCompany.IsEnabled = true;
                btnDeleteCompany.IsEnabled = true;
                btnEditCompany.Content = "Edit Product";
                btnDisconnect.IsEnabled = true;

                //to get into fillpath
                updateActive = false;

                //to make datagrid available again
                dgvCompanies.IsEnabled = true;
                tabLocked = false;
                tabNorthwind.IsEnabled = true;

                //--Update
                //Fill Company with values for SQLClass
                Company company = new Company(
                    int.Parse(txtIDCompany.Text), 
                    txtCompanyName.Text, 
                    txtContactName.Text, 
                    txtTitle.Text, 
                    txtAddress.Text, 
                    txtCity.Text, 
                    txtPostalCode.Text, 
                    txtCountry.Text, 
                    txtPhone.Text);
                
                //hand SQLClass the command and company class
                northwind.updateSqlCompany("update vw_simpleCompanyOverview " +
                    "set CompanyName = @CompanyName, " +
                    "ContactName = @ContactName, " +
                    "ContactTitle = @ContactTitle, " +
                    "[Address] = @Address, " +
                    "City = @City, " +
                    "PostalCode = @PostalCode, " +
                    "Country = @Country, " +
                    "Phone = @Phone " +
                    "where CompanyID = " + int.Parse(txtIDCompany.Text) +
                    ";", company);
            }
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            if (insertActive == false)
            {
                //Insert vorbereiten
                //lock tab and datagrid
                dgvCompanies.IsEnabled = false;
                tabLocked = true;
                tabNorthwind.IsEnabled = true;
                dgvCompanies.UnselectAll();

                //Hide ID & prepare for new ID
                txtIDCompany.Visibility = Visibility.Hidden;
                lblFixCompanyID.Visibility = Visibility.Hidden;
                txtIDCompany.Text = maxIDCompany.ToString();
                txtIDCompany.IsEnabled = false;

                //unlock form
                formLocked = false;
                txtCompanyName.IsReadOnly = false;
                txtContactName.IsReadOnly = false;
                txtTitle.IsReadOnly = false;
                txtAddress.IsReadOnly = false;
                txtCity.IsReadOnly = false;
                txtPostalCode.IsReadOnly = false;
                txtCountry.IsReadOnly = false;
                txtPhone.IsReadOnly = false;

                //disable/change Button
                btnAddCompany.Content = "Create Company";
                btnDeleteCompany.IsEnabled = false;
                btnEditCompany.IsEnabled = false;
                btnDisconnect.IsEnabled = false;

                //clear form from values
                txtCompanyName.Text = null;
                txtContactName.Text = null;
                txtTitle.Text = null;
                txtAddress.Text = null;
                txtCity.Text = null;
                txtPostalCode.Text = null;
                txtCountry.Text = null;
                txtPhone.Text = null;

                //get to insertpath
                insertActive = true;

            }
            else
            {
                {
                    //lock form 
                    formLocked = true;
                    txtIDCompany.IsEnabled = true;
                    txtIDCompany.Visibility = Visibility.Visible;
                    lblFixCompanyID.Visibility = Visibility.Visible;
                    txtCompanyName.IsReadOnly = true;
                    txtContactName.IsReadOnly = true;
                    txtTitle.IsReadOnly = true;
                    txtAddress.IsReadOnly = true;
                    txtCity.IsReadOnly = true;
                    txtPostalCode.IsReadOnly = true;
                    txtCountry.IsReadOnly = true;
                    txtPhone.IsReadOnly = true;
                }
                //unlock buttons
                btnEditCompany.IsEnabled = true;
                btnDeleteCompany.IsEnabled = true;
                btnDisconnect.IsEnabled = true;
                btnAddCompany.Content = "New Company";

                //leave insertpath
                insertActive = false;

                //add values if some are nonexistent
                if (string.IsNullOrEmpty(txtCompanyName.Text))
                {
                    txtCompanyName.Text = "Empty Company";
                }
                if (string.IsNullOrEmpty(txtContactName.Text))
                {
                    txtContactName.Text = "No Contact";
                }
                if (string.IsNullOrEmpty(txtTitle.Text))
                {
                    txtTitle.Text = "No Title";
                }
                if (string.IsNullOrEmpty(txtAddress.Text))
                {
                    txtAddress.Text = "No Address";
                }
                if (string.IsNullOrEmpty(txtCity.Text))
                {
                    txtCity.Text = "No City";
                }
                if (string.IsNullOrEmpty(txtPostalCode.Text))
                {
                    txtPostalCode.Text = "No Code";
                }
                if (string.IsNullOrEmpty(txtPhone.Text))
                {
                    txtPhone.Text = "No Phone";
                }
                if (string.IsNullOrEmpty(txtCountry.Text))
                {
                    txtCountry.Text = "No Country";
                }

                //fill Company with Values 
                Company company = new Company(
                    maxIDCompany,
                    txtCompanyName.Text,
                    txtContactName.Text,
                    txtTitle.Text,
                    txtAddress.Text,
                    txtCity.Text,
                    txtPostalCode.Text,
                    txtCountry.Text,
                    txtPhone.Text
                    );

                //Insertcommand
                tableCompanies = northwind.insertSqlCompany("Insert into vw_simpleCompanyOverview " +
                    "(CompanyName, ContactName, ContactTitle, [Address], City, PostalCode, Country, Phone) " +
                    "values(@CompanyName, @ContactName, @ContactTitle, @Address, @City, @PostalCode, @Country, @Phone)", company, tableCompanies);

                //enable datagrid and tab
                dgvCompanies.IsEnabled = true;
                tabLocked = true;
                tabNorthwind.IsEnabled = true;

                //Set next MaxID
                maxIDCompany = northwind.getMaxIDUniversal(tableCompanies);
                txtIDCompany.Text = maxIDCompany.ToString();

                //set selected item to maxID(new insertrow)
                dgvCompanies.SelectedIndex = maxIDCompany;
            }
        }

        private void btnDeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            //
            if (MessageBox.Show("Willst du wirklich die Firma \"" + txtCompanyName.Text + "\" mit der ID " + txtIDCompany.Text + " löschen ? ", Title = "Achtung!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DataRowView delRow = (DataRowView)dgvCompanies.SelectedItem;
                tableCompanies = northwind.deleteSqlRowUniversal("Delete from vw_simpleCompanyOverview where CompanyID = @ID", int.Parse(txtIDCompany.Text), tableCompanies, delRow);
            }
        }

        private void rdoFilterPAll_Checked(object sender, RoutedEventArgs e)
        {
            currentFilter = "all";
        }

        private void rdoFilterPGroupCategories_Checked(object sender, RoutedEventArgs e)
        {
            currentFilter = "groupCategories";
        }

        private void rdoFilterPGroupCompanies_Checked(object sender, RoutedEventArgs e)
        {
            currentFilter = "groupCompanies";
        }

        private void rdoFilterPoneCompany_Checked(object sender, RoutedEventArgs e)
        {
            currentFilter = "oneCompany";

            //activate given checkbox
            cboFilterCompany.IsEnabled = true;
        }
        private void rdoFilterPOneCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            //activate given checkbox
            cboFilterCompany.IsEnabled = false;
            cboFilterCompany.SelectedIndex = -1;
        }

        private void rdoFilterPoneCategory_Checked(object sender, RoutedEventArgs e)
        {
            currentFilter = "oneCategory";

            //activate given checkbox
            cboFilterCategory.IsEnabled = true;
        }
        private void rdoFilterPOneCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            //activate given checkbox
            cboFilterCategory.IsEnabled = false;
            cboFilterCategory.SelectedIndex = -1;
        }

        private void rdoFilterPPrice_Checked(object sender, RoutedEventArgs e)
        {
            currentFilter = "Price";
            grdPrice.IsEnabled = true;
            txtFilterBetween1.IsEnabled = false;
            txtFilterBetween2.IsEnabled = false;
            txtFilterAbove.IsEnabled = false;
            txtFilterUnder.IsEnabled = false;

        }
         private void rdoFilterPPrice_Unchecked(object sender, RoutedEventArgs e)
        {
            rdoFilterPPriceAbove.IsChecked = false;
            rdoFilterPPriceBetween.IsChecked = false;
            rdoFilterPPriceLess.IsChecked = false;
            grdPrice.IsEnabled = false;
        }

        private void rdoFilterPPriceLess_Checked(object sender, RoutedEventArgs e)
        {
            txtFilterUnder.IsEnabled = true;
        }
        private void rdoFilterPPriceLess_Unchecked(object sender, RoutedEventArgs e)
        {
            txtFilterUnder.IsEnabled = false;
            txtFilterUnder.Clear();
        }

        private void rdoFilterPPriceBetween_Checked(object sender, RoutedEventArgs e)
        {
            txtFilterBetween1.IsEnabled = true;
            txtFilterBetween2.IsEnabled = true;
        }
         private void rdoFilterPPriceBetween_Unchecked(object sender, RoutedEventArgs e)
        {
            txtFilterBetween1.IsEnabled = false;
            txtFilterBetween2.IsEnabled = false;
            txtFilterBetween1.Clear();
            txtFilterBetween2.Clear();
        }

        private void rdoFilterPPriceAbove_Checked(object sender, RoutedEventArgs e)
        {
            txtFilterAbove.IsEnabled = true;
        }

        private void rdoFilterPPriceAbove_Unchecked(object sender, RoutedEventArgs e)
        {
            txtFilterAbove.IsEnabled = false;
            txtFilterAbove.Clear();
        }

        private void cboFilterCompany_DropDownOpened(object sender, EventArgs e)
        {
            //lock dropdown
            if (currentFilter == "none")
            {
                cboFilterCompany.IsDropDownOpen = false;
            }
            else
            {
                cboFilterCompany.IsDropDownOpen = true;
            }
        }

        private void cboFilterCategory_DropDownOpened(object sender, EventArgs e)
        {
            //lock dropdown
            if (currentFilter == "none")
            {
                cboFilterCategory.IsDropDownOpen = false;
            }
            else
            {
                cboFilterCategory .IsDropDownOpen = true;
            }
        }

        private void btnFilterDeactivate_Click(object sender, RoutedEventArgs e)
        {
            grbProductFilter.IsEnabled = false;
        }

        private void btnFilterActivate_Click(object sender, RoutedEventArgs e)
        {
            grbProductFilter.IsEnabled = true;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            tableFilter = new DataTable();
            
            //Depending on clicked rdo: Fill Data into Table for Filter
            switch (currentFilter)
            {
                case "all":
                    dgvProducts.ItemsSource = tableProducts.DefaultView;
                    break;
                case "groupCompanies":
                    tableFilter = northwind.getSqlTableUniversal(
                        "Select CompanyName, " +
                        "count(ProductID) as Products " +
                        "from vw_simpleProductOverview " +
                        "where CompanyName is not null " +
                        "group by CompanyName", tableFilter);
                    dgvProducts.ItemsSource = tableFilter.DefaultView;
                    break;
                case "groupCategories":
                    tableFilter = northwind.getSqlTableUniversal(
                        "Select CategoryName, " +
                        "count(ProductID) as Products " +
                        "from vw_simpleProductOverview " +
                        "where CategoryName is not null " +
                        "group by CategoryName", tableFilter);
                    dgvProducts.ItemsSource = tableFilter.DefaultView;
                    break;
                case "oneCompany":
                    if (string.IsNullOrEmpty(cboFilterCompany.Text))
                    {
                        cboFilterCompany.SelectedIndex = randInt.Next(0, companyList.Count - 1);
                    }
                    tableFilter = northwind.getSqlTableUniversal(
                        "Select * " +
                        "from vw_simpleProductOverview " +
                        "where CompanyName = '" + northwind.correctStringForSql(cboFilterCompany.Text) + "'", tableFilter);
                    dgvProducts.ItemsSource = tableFilter.DefaultView;
                    break;
                case "oneCategory":
                    if (string.IsNullOrEmpty(cboFilterCategory.Text))
                    {
                        cboFilterCategory.SelectedIndex = randInt.Next(0, categoryList.Count - 1);
                    }
                    tableFilter = northwind.getSqlTableUniversal(
                        "Select * " +
                        "from vw_simpleProductOverview " +
                        "where CategoryName = '" + northwind.correctStringForSql(cboFilterCategory.Text) + "'", tableFilter);
                    dgvProducts.ItemsSource = tableFilter.DefaultView;
                    break;
                case "Price":
                    if (rdoFilterPPriceAbove.IsChecked == true)
                    {
                        if (string.IsNullOrEmpty(txtFilterAbove.Text))
                        {
                            txtFilterAbove.Text = "20";
                        }
                        tableFilter = northwind.getSqlTableUniversal(
                        "Select * from vw_simpleProductOverview where UnitPrice > "
                        + txtFilterAbove.Text, tableFilter);
                        dgvProducts.ItemsSource = tableFilter.DefaultView;
                    }
                    else
                    {
                        if (rdoFilterPPriceBetween.IsChecked == true)
                        {
                            if (string.IsNullOrEmpty(txtFilterBetween1.Text))
                            {
                                txtFilterBetween1.Text = "20";
                            }
                            if (string.IsNullOrEmpty(txtFilterBetween2.Text))
                            {
                                txtFilterBetween2.Text = "25";
                            }
                            tableFilter = northwind.getSqlTableUniversal(
                            "Select * from vw_simpleProductOverview where UnitPrice > "
                            + txtFilterBetween1.Text +
                            " and UnitPrice < "
                            + txtFilterBetween2.Text, tableFilter);
                            dgvProducts.ItemsSource = tableFilter.DefaultView;
                        }
                        else
                        {
                            if (rdoFilterPPriceLess.IsChecked == true)
                            {
                                if (string.IsNullOrEmpty(txtFilterUnder.Text))
                                {
                                    txtFilterUnder.Text = "20";
                                }
                                tableFilter = northwind.getSqlTableUniversal(
                                "Select * from vw_simpleProductOverview where UnitPrice < "
                                + txtFilterUnder.Text, tableFilter);
                                dgvProducts.ItemsSource = tableFilter.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("Bitte eine Option auswählen!", Title = "Achtung!");
                                return;
                            }

                        }
                    }
                    break;
            }
            grbProductFilter.IsEnabled = false;
        }

        private void grbProductFilter_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (startedConnNorthwind == true)
            {
                if (filterActive == false)
                {
                    //change Buttons
                    btnProductFilterActivate.IsEnabled = false;
                    btnProductFilterDeactivate.IsEnabled = true;
                    btnProductFilter.IsEnabled = true;

                    //change Radioboxes
                    rdoFilterPAll.IsEnabled = true;
                    rdoFilterPGroupCategories.IsEnabled = true;
                    rdoFilterPGroupCompanies.IsEnabled = true;
                    rdoFilterPOneCategory.IsEnabled = true;
                    rdoFilterPOneCompany.IsEnabled = true;
                    rdoFilterPPrice.IsEnabled = true;

                    //disable Northwind Datagrids 
                    tabNorthwind.IsEnabled = false;

                    filterActive = true;
                }
                else
                {
                    //change Buttons
                    btnProductFilterActivate.IsEnabled = true;
                    btnProductFilterDeactivate.IsEnabled = false;
                    btnProductFilter.IsEnabled = false;

                    //change Radioboxes
                    rdoFilterPAll.IsEnabled = false;
                    rdoFilterPGroupCategories.IsEnabled = false;
                    rdoFilterPGroupCompanies.IsEnabled = false;
                    rdoFilterPOneCategory.IsEnabled = false;
                    rdoFilterPOneCompany.IsEnabled = false;
                    rdoFilterPPrice.IsEnabled = false;

                    //enable Northwind Datagrids again
                    tabNorthwind.IsEnabled = true;

                    //Deactivate currently picked radiobox
                    switch (currentFilter)
                    {
                        case "all":
                            rdoFilterPAll.IsChecked = false;
                            break;
                        case "groupCompanies":
                            rdoFilterPGroupCompanies.IsChecked = false;
                            break;
                        case "groupCategories":
                            rdoFilterPGroupCategories.IsChecked = false;
                            break;
                        case "oneCompany":
                            rdoFilterPOneCompany.IsChecked = false;
                            break;
                        case "oneCategory":
                            rdoFilterPOneCategory.IsChecked = false;
                            break;
                        case "Price":
                            rdoFilterPPrice.IsChecked = false;
                            break;
                    }

                    currentFilter = "none";

                    filterActive = false;
                }
            }
        }
    }
}
