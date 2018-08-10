using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Export
{
    [XmlType("AnimalAid")]
    public class ExportAnimalAidDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }

        [XmlElement("Price")]
        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
