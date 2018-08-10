using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.App.Dtos.Import
{
    [XmlType("customer")]
    public class CustomertDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("birth-date")]
        public string BirthDate { get; set; }

        [XmlElement("is-young-driver")]
        public bool IsYoungDriver { get; set; }
    }
}
