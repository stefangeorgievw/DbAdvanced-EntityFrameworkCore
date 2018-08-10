using PetClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("Procedure")]
    public class ProcedureDto
    {
        [XmlElement("Vet")]
        [Required]
        public string Vet { get; set; }

        [XmlElement("Animal")]
        [Required]
        public string AnimalSerialNumber { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public AnimalAidProcedureDto[] AnimalAids { get; set; }
    }
}
