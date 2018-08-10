using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.App.DTOs.Export
{
    [XmlType("sold-products ")]
    public class SoldProductsExportDto
    {
        [XmlAttribute("count")]
        public long Count { get; set; }

        [XmlElement("product")]
        public ProductByUserExportDto[] Products { get; set; }
    }
}
