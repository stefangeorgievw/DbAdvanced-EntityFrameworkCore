
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MinLength(3)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int? BuyerId { get; set; }
        public User Buyer { get; set; }

        public int SellerId { get; set; }
        public User Seller { get; set; }

        public ICollection<CategoryProducts> CategoryProducts => new List<CategoryProducts>();
    }
}
