using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.App.DTOs.Export
{
    [XmlType("product")]
    public class ProductsInRangeDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [XmlAttribute("buyer")]
        public string Buyer { get; set; }
    }
}
