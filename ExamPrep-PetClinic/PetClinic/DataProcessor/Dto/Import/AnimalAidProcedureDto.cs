using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("AnimalAid")]
    public class AnimalAidProcedureDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }
    }
}
