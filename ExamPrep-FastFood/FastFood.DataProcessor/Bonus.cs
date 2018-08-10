using System;
using System.Linq;
using FastFood.Data;

namespace FastFood.DataProcessor
{
    public static class Bonus
    {
        public static string UpdatePrice(FastFoodDbContext context, string itemName, decimal newPrice)
        {
            var item = context.Items.Where(x => x.Name == itemName).SingleOrDefault();

            if (item == null)
            {
                return $"Item {itemName} not found!";

            }

            var oldPrice = item.Price;

            item.Price = newPrice;

            return $"{itemName} Price updated from ${oldPrice:F2} to ${newPrice:F2}";



        }
    }
}
