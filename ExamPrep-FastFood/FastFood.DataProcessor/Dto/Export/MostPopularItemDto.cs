using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Export
{
    
    public class MostPopularItemDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }

        [XmlElement("TotalMade")]
        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal TotalMade { get; set; }


        [XmlElement("TimesSold")]
        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal TimesSold { get; set; }
    }
}
