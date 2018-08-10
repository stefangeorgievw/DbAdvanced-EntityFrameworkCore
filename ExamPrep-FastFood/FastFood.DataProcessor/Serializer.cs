using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Export;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var employee = context.Employees.Where(e => e.Name == employeeName)
                .ToArray()
                                            .Select(e => new
                                            {
                                                Name = e.Name,

                                                Orders = e.Orders.Where(x => x.Type.ToString() == orderType).Select(x => new
                                                {
                                                    Customer = x.Customer,
                                                    Items = x.OrderItems.Select(or => new
                                                    {
                                                        Name = or.Item.Name,
                                                        Price = or.Item.Price,
                                                        Quantity = or.Quantity
                                                    }).ToArray(),

                                                    TotalPrice = x.TotalPrice

                                                }).OrderByDescending(x => x.TotalPrice)
                                                .ThenByDescending(x => x.Items.Count())
                                                .ToArray()
                                                ,

                                                TotalMade = e.Orders.Where(x => x.Type.ToString() == orderType).Sum(x => x.TotalPrice)


                                            }).SingleOrDefault();

            var jsonString = JsonConvert.SerializeObject(employee, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categoriesArray = categoriesString.Split(',');

            var categories = context.Categories.Where(c => categoriesArray.Any(s => s == c.Name))
                                               .Select(c => new CategoryDto
                                               {
                                                   Name = c.Name,
                                                   MostPopularItem = c.Items.Select(i=>  new MostPopularItemDto
                                                   {
                                                       Name = i.Name,
                                                       TotalMade = i.OrderItems.Sum(x=> x.Item.Price * x.Quantity),
                                                       TimesSold = i.OrderItems.Sum(x=> x.Quantity)
                                                   })
                                                    .OrderByDescending(x => x.TotalMade)
                                                    .ThenByDescending(x => x.TimesSold)
                                                   .FirstOrDefault()
                                               })
                                               .OrderByDescending(x => x.MostPopularItem.TotalMade)
                                               .ThenByDescending(x => x.MostPopularItem.TimesSold)
                                               .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var selializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("Categories"));
            selializer.Serialize(new StringWriter(sb), categories, xmlNamespaces);

            return sb.ToString();
        }
    }
}