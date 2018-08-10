using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.App.DTOs.Export
{
    [XmlRoot("users")]
    public class UserExportDto
    {
        [XmlAttribute("count")]
        public long Count { get; set; }

        [XmlElement("user")]
        public UserAndProductsDto[] Users { get; set; }
    }
}
