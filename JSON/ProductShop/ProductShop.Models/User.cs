using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductShop.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        public string LastName { get; set; }

        public int? Age { get; set; }

        public ICollection<Product> BoughtProducts => new List<Product>();

        public ICollection<Product> SelledProducts => new List<Product>();
    }
}
