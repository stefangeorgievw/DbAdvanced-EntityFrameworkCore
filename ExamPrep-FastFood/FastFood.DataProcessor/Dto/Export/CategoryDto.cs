using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Export
{
    [XmlType("Category")]
    public class CategoryDto
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [XmlElement("MostPopularItem")]
        public MostPopularItemDto MostPopularItem { get; set; }


    }
}
