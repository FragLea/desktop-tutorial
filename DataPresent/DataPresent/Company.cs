using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPresent
{
    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string contactName { get; set; }
        public string contactTitle { get; set; }
        public string address { get; set;}
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }

        public Company(string name)
        {
            Name = name;
        }

        public Company(
            int iD,
            string name,
            string contactName,
            string contactTitle,
            string adress,
            string city,
            string postalCode,
            string country,
            string phone)
        {   
            ID = iD;
            Name = name;
            this.contactName = contactName;
            this.contactTitle = contactTitle;
            this.address = adress;
            this.city = city;
            this.postalCode = postalCode;
            this.country = country;
            this.phone = phone;
        }
    }
}
