using FastFood.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FastFood.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string Customer { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public OrderType Type { get; set; } = OrderType.ForHere;

        [Required]
        [NotMapped]
        public decimal TotalPrice => this.OrderItems.Sum(x => x.Item.Price * x.Quantity);

        public int EmployeeId { get; set; }
        [Required]
        public Employee Employee { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


    }
}