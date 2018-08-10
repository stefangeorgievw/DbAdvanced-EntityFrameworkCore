using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.App.DTOs.Export
{
    [XmlType("user")]
    public class UserSoldProducts
    {
        [XmlAttribute("first-name")]
        public string FirstName { get; set; }

        [XmlAttribute("last-name")]
        public string LastName { get; set; }

        [XmlArray("sold-products")]
        public SoldProduct[] SoldProduct { get; set; }
    }
}
