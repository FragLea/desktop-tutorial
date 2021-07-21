using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataPresent
{
    class Northwind
    {
        private string connectionString = "Data Source=MD2SBDCC\\TESTINGSERVER;Initial Catalog=Northwind;Integrated Security=True";
        SqlConnection connection = new SqlConnection();
        SqlCommand command = new SqlCommand();
        SqlDataAdapter adaptTable;
        SqlDataReader reader;
        DataTable table = new DataTable();

        public Northwind()
        {
            
        }
        public void startConnection()
        {
            //start up the connection to northwind
            connection.ConnectionString = connectionString;
            connection.Open();
            command.Connection = connection;
        }
        public DataTable getSqlTableUniversal(string sqlCommandString, DataTable table)
        {

            //hand command to data
            adaptTable = new SqlDataAdapter(sqlCommandString, connection);
            adaptTable.Fill(table);
            
            //clear command
            command.CommandText = null;
            return table;
        }

        public List<Category> getCategorySqlList(string sqlCommandString, List<Category> categoryList)
        {
            //for filling the Categorylist 
            command.CommandText = sqlCommandString;
            using (reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //foreach read category fill it into the list
                    categoryList.Add(new Category(reader["CategoryName"].ToString()));
                }
            }

            //clear command
            command.CommandText = null;
            return categoryList;
        }

        public List<Company> getCompanySqlList(string sqlCommandString, List<Company> companyList)
        {
            //for filling the Companylist
            command.CommandText = sqlCommandString;
            using (reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //foreach read category fill it into the list
                    companyList.Add(new Company(
                        (int)reader["CompanyID"],
                        reader["CompanyName"].ToString(),
                        reader["ContactName"].ToString(),
                        reader["ContactTitle"].ToString(),
                        reader["Address"].ToString(),
                        reader["City"].ToString(),
                        reader["PostalCode"].ToString(),
                        reader["Country"].ToString(),
                        reader["Phone"].ToString()));
                }
            }
            //clear command
            command.CommandText = null;
            return companyList;
        }

        public void updateSqlProduct(string sqlCommandString, Product product)
        {
            //update-function for a product
            command.CommandText = sqlCommandString;

            //add parameters to the CommandString
            command.Parameters.Add(new SqlParameter("ProductName", product.name));
            command.Parameters.Add(new SqlParameter("CompanyName", product.company));
            command.Parameters.Add(new SqlParameter("CategoryName", product.category));
            command.Parameters.Add(new SqlParameter("QuantityPerUnit", product.quantityPerUnit));
            command.Parameters.Add(new SqlParameter("UnitPrice", product.unitPrice));
            command.Parameters.Add(new SqlParameter("UnitsInStock", product.unitsInStock));
            command.Parameters.Add(new SqlParameter("Discontinued", product.discontinued));
            
            //do the update
            command.ExecuteNonQuery();
            
            //delete CommandString    
            command.CommandText = null;
            
            //delete Parameters(if not: error on second walkthrough)
            command.Parameters.RemoveAt("ProductName");
            command.Parameters.RemoveAt("CompanyName");
            command.Parameters.RemoveAt("CategoryName");
            command.Parameters.RemoveAt("QuantityPerUnit");
            command.Parameters.RemoveAt("UnitPrice");
            command.Parameters.RemoveAt("UnitsInStock");
            command.Parameters.RemoveAt("Discontinued");
        }

        public void updateSqlCompany(string sqlCommandString, Company company)
        {
            //update-function for company
            command.CommandText = sqlCommandString;

            //add parameters to the CommandString
            command.Parameters.Add(new SqlParameter("CompanyName", company.Name));
            command.Parameters.Add(new SqlParameter("ContactName", company.contactName));
            command.Parameters.Add(new SqlParameter("ContactTitle", company.contactTitle));
            command.Parameters.Add(new SqlParameter("Address", company.address));
            command.Parameters.Add(new SqlParameter("City", company.city));
            command.Parameters.Add(new SqlParameter("PostalCode", company.postalCode));
            command.Parameters.Add(new SqlParameter("Country", company.country));
            command.Parameters.Add(new SqlParameter("Phone", company.phone));
            
            //do the update
            command.ExecuteNonQuery();
            
            //delete CommandString
            command.CommandText = null;
            
            //Delete Parameters(if not: error on second walkthrough)
            command.Parameters.RemoveAt("CompanyName");
            command.Parameters.RemoveAt("ContactName");
            command.Parameters.RemoveAt("ContactTitle");
            command.Parameters.RemoveAt("Address");
            command.Parameters.RemoveAt("City");
            command.Parameters.RemoveAt("PostalCode");
            command.Parameters.RemoveAt("Country");
            command.Parameters.RemoveAt("Phone");
        }

        public DataTable insertSqlProduct(string sqlCommandString, Product product, DataTable table) 
        {
            //create new row for table
            DataRow addInsert = table.NewRow();
            
            //Use update for inserting into table(same result)
            updateSqlProduct(sqlCommandString, product);
            
            //add values to row
            addInsert["ProductID"] = product.ID;
            addInsert["ProductName"] = product.name;
            addInsert["CompanyName"] = product.company;
            addInsert["CategoryName"] = product.category;
            addInsert["QuantityPerUnit"] = product.quantityPerUnit;
            addInsert["UnitPrice"] = product.unitPrice;
            addInsert["UnitsInStock"] = product.unitsInStock;
            addInsert["Discontinued"] = product.discontinued;
            
            //add row to table
            table.Rows.Add(addInsert);

            //delete row
            addInsert.Delete();

            return table;
        }

        public DataTable insertSqlCompany(string sqlCommandString, Company company, DataTable table)
        {
            //create new row for table
            DataRow addInsert = table.NewRow();

            //Use update for inserting into table(same result)
            updateSqlCompany(sqlCommandString, company);

            //add values to row
            addInsert["CompanyID"] = company.ID;
            addInsert["Companyname"] = company.Name;
            addInsert["ContactName"] = company.contactName;
            addInsert["ContactTitle"] = company.contactTitle;
            addInsert["Address"] = company.address;
            addInsert["City"] = company.city;
            addInsert["PostalCode"] = company.postalCode;
            addInsert["Country"] = company.country;
            addInsert["Phone"] = company.phone;
            

            //add row to table
            table.Rows.Add(addInsert);

            //delete row
            addInsert.Delete();

            return table;
        }

        public DataTable deleteSqlRowUniversal(string sqlCommandString, int ID, DataTable table, DataRowView delRow)
        { 
            //universal function to delete row from table and database
            //sqlpart
            command = new SqlCommand(sqlCommandString, connection);
            command.Parameters.Add(new SqlParameter("ID", ID));
            command.ExecuteNonQuery();
            command.CommandText = null;
            command.Parameters.RemoveAt("ID");
            //tablepart
            table.Rows.Remove(delRow.Row);

            return table;
        }
        
        public int getMaxIDUniversal(DataTable table)
        {
            //universal function to get maxID from table
            int returnInt = (int)table.Rows.Count + 1;
            return returnInt;
        }

        public string correctStringForSql(string s)
        {
            Regex regex = new Regex("^.*'.*$");

            if (regex.IsMatch(s))
            {
                s = s.Replace("'", "''");
            }

            return s;
        }

        public void endConenction()
        {   
            //end Connection to Northwind
            connection.Close();
            connection.Dispose();
        }
    }
}
