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
        public bool startedConnNorthwind { get; private set; } = false;       //to make sure there is a connection to close/not another connection running
        public bool updateActive = false;               //to make sure you can't accidentaly change/enter anything! 
        public bool insertActive = false;               //-||-
        public bool deleteActive = false;               //-||-
        public bool formLocked = true;                  //to lock the form except for when user needs to edit it. false locks ID
        public bool tabLocked = true;
        public bool filterProductActive = false;
        public bool filterCompanyActive = false;
        public string currentProductFilter = "none";
        public string currentCompanyFilter = "none";
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
            this.DataContext = this;
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

                foreach (Company c in companyList)
                {
                    if (!cboCountries.Items.Contains(c.country))
                    {
                        cboCountries.Items.Add(c.country);
                    }
                }

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
                grbCompanyFilter.Visibility = Visibility.Hidden;
                btnFilterActivate.Visibility = Visibility.Visible;
                btnFilterActivate.IsEnabled = true;
                btnFilterDeactivate.Visibility = Visibility.Visible;
                btnFilterDeactivate.IsEnabled = false;
                btnFilter.Visibility = Visibility.Visible;
                btnFilter.IsEnabled = false;
                filterProductActive = false;
                filterCompanyActive = false;

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

                    cboCountries.Items.Clear();

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
                btnFilterActivate.IsEnabled = false;
                //product
                btnInsert.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                btnDelete.IsEnabled = false;
                
                //Company
                btnAddCompany.IsEnabled = false;
                btnEditCompany.IsEnabled = false;
                btnDeleteCompany.IsEnabled = false;

                //Hide/Disable groupboxes
                tabNorthwind.IsEnabled = false;
                grbProductDataRow.Visibility = Visibility.Hidden;
                grbCompanyDataRow.Visibility = Visibility.Hidden;
                grbProductFilter.Visibility = Visibility.Hidden;
                grbCompanyFilter.Visibility = Visibility.Hidden;
                btnFilterActivate.Visibility = Visibility.Hidden;
                btnFilterDeactivate.Visibility = Visibility.Hidden;
                btnFilter.Visibility = Visibility.Hidden;
                filterProductActive = false;
                filterCompanyActive = false;
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
                    "\n-filterProductActive: " + filterProductActive +
                    "\n-currentProductFilter: " + currentProductFilter +
                    "\n-filterCompanyActive: " + filterCompanyActive +
                    "\n-currentCompanyFilter: " + currentCompanyFilter +
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
                btnFilterActivate.IsEnabled = false;
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
                btnFilterActivate.IsEnabled = true;
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
                btnFilterActivate.IsEnabled = false;
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
                btnFilterActivate.IsEnabled = true;
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
                    cboCategory.SelectedItem = randInt.Next(cboCategory.Items.Count) - 1;
                }
                if (string.IsNullOrEmpty(cboCompany.Text))
                {
                    cboCategory.SelectedItem = randInt.Next(cboCompany.Items.Count) - 1;
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

                    //Show right datagrid
                    dgvProducts.IsEnabled = true;
                    dgvCompanies.IsEnabled = false;
                    dgvProducts.SelectedItem = currentIDProduct;

                    grbProductFilter.Visibility = Visibility.Visible;
                    grbCompanyFilter.Visibility = Visibility.Hidden;

                }
            }
            if (TabCompany.IsSelected)
            {
                //When user picks Companytab
                if (startedConnNorthwind == true)
                {
                    //Show right form
                    grbCompanyDataRow.Visibility = Visibility.Visible;
                    grbProductDataRow.Visibility = Visibility.Hidden;

                    //Show right datagrid
                    dgvCompanies.IsEnabled = true;
                    dgvProducts.IsEnabled = false;
                    dgvCompanies.SelectedItem = currentIDCompany;

                    //Show right filters
                    grbProductFilter.Visibility = Visibility.Hidden;
                    grbCompanyFilter.Visibility = Visibility.Visible;

                }
            }
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
            currentProductFilter = "all";
        }

        private void rdoFilterPGroupCategories_Checked(object sender, RoutedEventArgs e)
        {
            currentProductFilter = "groupCategories";
        }

        private void rdoFilterPGroupCompanies_Checked(object sender, RoutedEventArgs e)
        {
            currentProductFilter = "groupCompanies";
        }

        private void rdoFilterPoneCompany_Checked(object sender, RoutedEventArgs e)
        {
            currentProductFilter = "oneCompany";

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
            currentProductFilter = "oneCategory";

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
            currentProductFilter = "Price";
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
            if (currentProductFilter == "none")
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
            if (currentProductFilter == "none")
            {
                cboFilterCategory.IsDropDownOpen = false;
            }
            else
            {
                cboFilterCategory.IsDropDownOpen = true;
            }
        }

        private void btnFilterDeactivate_Click(object sender, RoutedEventArgs e)
        {
            if (TabProduct.IsSelected && filterProductActive == true)
            {
                grbProductFilter.IsEnabled = false;
            }
            else if (TabCompany.IsSelected && filterCompanyActive == true)
            {
                grbCompanyFilter.IsEnabled = false;
            }
        }

        private void btnFilterActivate_Click(object sender, RoutedEventArgs e)
        {
            if (TabProduct.IsSelected && filterProductActive == false)
            {
                grbProductFilter.IsEnabled = true;
            }
            else if (TabCompany.IsSelected && filterCompanyActive == false)
            {
                grbCompanyFilter.IsEnabled = true;
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            tableFilter = new DataTable();
            if (filterProductActive == true)
            {
                //Depending on clicked rdo: Fill Data into Table for Filter
                switch (currentProductFilter)
                {
                    case "all":
                        //back to old table
                        dgvProducts.ItemsSource = tableProducts.DefaultView;
                        break;
                    case "groupCompanies":
                        //Count Products per Company
                        tableFilter = northwind.getSqlTableUniversal(
                            "Select CompanyName, " +
                            "count(ProductID) as Products " +
                            "from vw_simpleProductOverview " +
                            "where CompanyName is not null " +
                            "group by CompanyName", tableFilter);
                        dgvProducts.ItemsSource = tableFilter.DefaultView;
                        break;
                    case "groupCategories":
                        //Count Products per Category
                        tableFilter = northwind.getSqlTableUniversal(
                            "Select CategoryName, " +
                            "count(ProductID) as Products " +
                            "from vw_simpleProductOverview " +
                            "where CategoryName is not null " +
                            "group by CategoryName", tableFilter);
                        dgvProducts.ItemsSource = tableFilter.DefaultView;
                        break;
                    case "oneCompany":
                        //show all products to this Company
                        if (string.IsNullOrEmpty(cboFilterCompany.Text))
                        {
                            //random Company if none chosen
                            cboFilterCompany.SelectedIndex = randInt.Next(0, companyList.Count - 1);
                        }
                        tableFilter = northwind.getSqlTableUniversal(
                            "Select * " +
                            "from vw_simpleProductOverview " +
                            "where CompanyName = '" + northwind.correctStringForSql(cboFilterCompany.Text) + "'", tableFilter);
                        dgvProducts.ItemsSource = tableFilter.DefaultView;
                        break;
                    case "oneCategory":
                        //show all products to this Category
                        if (string.IsNullOrEmpty(cboFilterCategory.Text))
                        {
                            //random Category if none chosen
                            cboFilterCategory.SelectedIndex = randInt.Next(0, categoryList.Count - 1);
                        }
                        tableFilter = northwind.getSqlTableUniversal(
                            "Select * " +
                            "from vw_simpleProductOverview " +
                            "where CategoryName = '" + northwind.correctStringForSql(cboFilterCategory.Text) + "'", tableFilter);
                        dgvProducts.ItemsSource = tableFilter.DefaultView;
                        break;
                    case "Price":
                        //Show Products related to a given price.
                        if (rdoFilterPPriceAbove.IsChecked == true)
                        {
                            //above a value
                            if (string.IsNullOrEmpty(txtFilterAbove.Text))
                            {
                                //if none is given
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
                                //between 2 value
                                if (string.IsNullOrEmpty(txtFilterBetween1.Text))
                                {
                                    //if none is given
                                    txtFilterBetween1.Text = "20";
                                }
                                if (string.IsNullOrEmpty(txtFilterBetween2.Text))
                                {
                                    //if none is given
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
                                    //below a value
                                    if (string.IsNullOrEmpty(txtFilterUnder.Text))
                                    {
                                        //if none is given
                                        txtFilterUnder.Text = "20";
                                    }
                                    tableFilter = northwind.getSqlTableUniversal(
                                    "Select * from vw_simpleProductOverview where UnitPrice < "
                                    + txtFilterUnder.Text, tableFilter);
                                    dgvProducts.ItemsSource = tableFilter.DefaultView;
                                }
                                else
                                {
                                    //
                                    MessageBox.Show("Bitte eine Option auswählen!", Title = "Achtung!");
                                    return;
                                }

                            }
                        }
                        break;
                }
                grbProductFilter.IsEnabled = false;
            }
            else if (filterCompanyActive == true)
            {

                switch (currentCompanyFilter)
                {
                    case "all":
                        dgvCompanies.ItemsSource = tableCompanies.DefaultView;
                        break;
                    case "groupCountries":
                        tableFilter = northwind.getSqlTableUniversal(
                            "Select Country, count(CompanyID) as Companies " +
                            "from vw_simpleCompanyOverview " +
                            "where Country is not null " +
                            "group by Country"
                            , tableFilter);
                        dgvCompanies.ItemsSource = tableFilter.DefaultView;
                        break;
                    case "oneCountry":
                        if (string.IsNullOrEmpty(cboCountries.Text))
                        {
                            //random Category if none chosen
                            cboCountries.SelectedIndex = randInt.Next(0, categoryList.Count - 1);
                        }
                        tableFilter = northwind.getSqlTableUniversal(
                            "select * from tfn_lookUpCountry('" + northwind.correctStringForSql(cboCountries.Text) + "')"
                            , tableFilter);
                        dgvCompanies.ItemsSource = tableFilter.DefaultView;
                        break;
                    case "companyLetter":
                        string nameLike;
                        if (string.IsNullOrEmpty(txtFilterCLetters.Text))
                        {
                            nameLike = "exo";
                        }
                        else
                        {
                            nameLike = northwind.correctStringForSql(txtFilterCLetters.Text);
                        }
                        if (rdoFilterCompanyAny.IsChecked == true)
                        {
                            nameLike = "%" + nameLike + "%";
                        }
                        else if (rdoFilterCompanyMiddle.IsChecked == true)
                        {
                            nameLike = "%_" + nameLike + "_%";
                        }
                        else if (rdoFilterCompanyBegin.IsChecked == true)
                        {
                            nameLike = nameLike + "%";
                        }
                        else if (rdoFilterCompanyEnd.IsChecked == true)
                        {
                            nameLike = "%" + nameLike;
                        }
                        tableFilter = northwind.getSqlTableUniversal(
                            "Select * from vw_simpleCompanyOverview where CompanyName like '" + nameLike + "'"
                            , tableFilter);
                        dgvCompanies.ItemsSource = tableFilter.DefaultView;
                        MessageBox.Show("Results for search: \"" + txtFilterCLetters.Text + "\"");
                        break;
                    case "companyLength":
                        string sortBy = " asc";
                        
                        if (rdoFilterCompanySortDesc.IsChecked == true)
                        {
                            sortBy = " desc";
                        }
                        tableFilter = northwind.getSqlTableUniversal(
                            "Select *, len(CompanyName) as Namelenght from vw_simpleCompanyOverview order by Namelenght" + sortBy
                            , tableFilter);
                        dgvCompanies.ItemsSource = tableFilter.DefaultView;
                        break;
                }

                grbCompanyFilter.IsEnabled = false;
            }
        }

        private void grbProductFilter_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (startedConnNorthwind == true)
            {
                if (filterProductActive == false)
                {
                    //change Buttons
                    btnFilterActivate.IsEnabled = false;
                    btnFilterDeactivate.IsEnabled = true;
                    btnFilter.IsEnabled = true;

                    //change Radioboxes
                    rdoFilterPAll.IsEnabled = true;
                    rdoFilterPGroupCategories.IsEnabled = true;
                    rdoFilterPGroupCompanies.IsEnabled = true;
                    rdoFilterPOneCategory.IsEnabled = true;
                    rdoFilterPOneCompany.IsEnabled = true;
                    rdoFilterPPrice.IsEnabled = true;

                    //disable Northwind Datagrids 
                    tabNorthwind.IsEnabled = false;

                    filterProductActive = true;
                }
                else
                {
                    //change Buttons
                    btnFilterActivate.IsEnabled = true;
                    btnFilterDeactivate.IsEnabled = false;
                    btnFilter.IsEnabled = false;

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
                    switch (currentProductFilter)
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
                    //put filter back in old state
                    currentProductFilter = "none";

                    filterProductActive = false;
                }
            }
        }

        private void grbCompanyFilter_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (startedConnNorthwind == true)
            {
                if (filterCompanyActive == false)
                {
                    //change Buttons
                    btnFilterActivate.IsEnabled = false;
                    btnFilterDeactivate.IsEnabled = true;
                    btnFilter.IsEnabled = true;

                    //change Radioboxes/groupboxes
                    rdoFilterCAll.IsEnabled = true;
                    rdoFilterCCompaniesContaining.IsEnabled = true;
                    rdoFilterCLookUpCountry.IsEnabled = true;
                    rdoFilterCPerCountry.IsEnabled = true;
                    rdoFilterCSortByNameLenght.IsEnabled = true;

                    grdCSort.IsEnabled = false;
                    grdFilterCNameWithLetters.IsEnabled = false;
                    cboCountries.IsEnabled = false;

                    //disable Northwind Datagrids 
                    tabNorthwind.IsEnabled = false;

                    filterCompanyActive = true;
                }
                else
                {
                    //change Buttons
                    btnFilterActivate.IsEnabled = true;
                    btnFilterDeactivate.IsEnabled = false;
                    btnFilter.IsEnabled = false;

                    //change Radioboxes
                    rdoFilterCAll.IsEnabled = false;
                    rdoFilterCCompaniesContaining.IsEnabled = false;
                    rdoFilterCLookUpCountry.IsEnabled = false;
                    rdoFilterCPerCountry.IsEnabled = false;
                    rdoFilterCSortByNameLenght.IsEnabled = false;
                    

                    grdCSort.IsEnabled = false;
                    grdFilterCNameWithLetters.IsEnabled = false;
                    cboCountries.IsEnabled = false;

                    //enable Northwind Datagrids again
                    tabNorthwind.IsEnabled = true;

                    //Deactivate currently picked radiobox
                    switch (currentCompanyFilter)
                    {
                        case "all":
                            rdoFilterCAll.IsChecked = false;
                            break;
                        case "groupCountries":
                            rdoFilterCPerCountry.IsChecked = false;
                            break;
                        case "oneCountry":
                            rdoFilterCLookUpCountry.IsChecked = false;
                            break;
                        case "companyLetter":
                            rdoFilterCCompaniesContaining.IsChecked = false;
                            break;
                        case "companyLength":
                            rdoFilterCSortByNameLenght.IsChecked = false;
                            break;
                    }
                    //put filter back in old state
                    currentCompanyFilter = "none";

                    filterCompanyActive = false;
                }
            }
        }

        private void rdoFilterCAll_Checked(object sender, RoutedEventArgs e)
        {
            currentCompanyFilter = "all";
        }

        private void rdoFilterCPerCountry_Checked(object sender, RoutedEventArgs e)
        {
            currentCompanyFilter = "groupCountries";
        }

        private void rdoFilterCLookUpCountry_Checked(object sender, RoutedEventArgs e)
        {
            currentCompanyFilter = "oneCountry";
            cboCountries.IsEnabled = true;
        }

        private void rdoFilterCLookUpCountry_Unchecked(object sender, RoutedEventArgs e)
        {
            cboCountries.SelectedIndex = -1;
            cboCountries.IsEnabled = false;
        }

        private void txtbCompaniesContaining_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (rdoFilterCCompaniesContaining.IsChecked == false)
            {
                rdoFilterCCompaniesContaining.IsChecked = true;
            }
        }

        private void rdoFilterCCompaniesContaining_Checked(object sender, RoutedEventArgs e)
        {
            currentCompanyFilter = "companyLetter";
            grdFilterCNameWithLetters.IsEnabled = true;
            rdoFilterCompanyAny.IsChecked = true;
        }

        private void rdoFilterCCompaniesContaining_Unchecked(object sender, RoutedEventArgs e)
        {
            currentCompanyFilter = "none";
            grdFilterCNameWithLetters.IsEnabled = false;
            rdoFilterCompanyAny.IsChecked = false;
            rdoFilterCompanyBegin.IsChecked = false;
            rdoFilterCompanyEnd.IsChecked = false;
            rdoFilterCompanyMiddle.IsChecked = false;
            txtFilterCLetters.Text = "";
        }

        private void rdoFilterCSortByNameLenght_Checked(object sender, RoutedEventArgs e)
        {
            currentCompanyFilter = "companyLength";
            grdCSort.IsEnabled = true;
            rdoFilterCompanySortAsc.IsChecked = true;
        }

        private void rdoFilterCSortByNameLenght_Unchecked(object sender, RoutedEventArgs e)
        {
            currentCompanyFilter = "none";
            grdCSort.IsEnabled = false;
            rdoFilterCompanySortAsc.IsChecked = false;
            rdoFilterCompanySortDesc.IsChecked = false;
        }
    }
}


