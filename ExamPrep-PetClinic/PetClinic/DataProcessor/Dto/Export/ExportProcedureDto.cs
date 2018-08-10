using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Export
{
    [XmlType("Procedure")]
    public class ExportProcedureDto
    {
        [XmlElement("Passport")]
        [RegularExpression("^[a-zA-Z]{7}[0-9]{3}$")]
        public string SerialNumber { get; set; }

        [XmlElement("OwnerNumber")]
        [Required]
        [RegularExpression("^\\+359[0-9]{9}|0[0-9]{9}$")]
        public string OwnerPhoneNumber { get; set; }

        [XmlElement("DateTime")]
        [Required]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public ExportAnimalAidDto[] AnimalAids { get; set; }

        [XmlElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
