using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPresent
{
    class Product
    {
        public int ID;
        public string name;
        public string company;
        public string category;
        public string quantityPerUnit;
        public decimal unitPrice;
        public int unitsInStock;
        public bool discontinued;

        public Product(int ID, 
            string name, 
            string company, 
            string category, 
            string quantityPerUnit,
            decimal unitPrice, 
            int unitsInStock, 
            bool discontinued)
        {
            this.ID = ID;
            this.name = name;
            this.company = company;
            this.category = category;
            this.quantityPerUnit = quantityPerUnit;
            this.unitPrice = unitPrice;
            this.unitsInStock = unitsInStock;
            this.discontinued = discontinued;
        }
    }
}
