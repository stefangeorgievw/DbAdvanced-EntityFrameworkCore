using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Import;
using FastFood.Models;
using FastFood.Models.Enums;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
	public static class Deserializer
	{
		private const string FailureMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

		public static string ImportEmployees(FastFoodDbContext context, string jsonString)
		{
            var desialised = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);
            var sb = new StringBuilder();

            var employees = new List<Employee>();
            foreach (var employeeDto in desialised)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
                var position = context.Positions.FirstOrDefault(x => x.Name == employeeDto.Position);
                if (position == null)
                {
                    position = new Position
                    {
                        Name = employeeDto.Position
                    };

                    context.Positions.Add(position);
                    context.SaveChanges();
                }

                var employee = new Employee
                {
                    Name = employeeDto.Name,
                    Age = employeeDto.Age,
                    Position = position
                };

                employees.Add(employee);
                sb.AppendLine(string.Format(SuccessMessage, employee.Name));
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
		}

		public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
            var desializedItemsDto = JsonConvert.DeserializeObject<ItemDto[]>(jsonString);
            var sb = new StringBuilder();

            var items = new List<Item>();

            foreach (var itemDto in desializedItemsDto)
            {
                var itemExists = items.Any(i => i.Name == itemDto.Name);
                if (!IsValid(itemDto) || itemExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var category = context.Categories.FirstOrDefault(x => x.Name == itemDto.Category);
                if (category == null)
                {
                    category = new Category
                    {
                        Name = itemDto.Category
                    };

                    context.Categories.Add(category);
                    context.SaveChanges();
                }

                var item = new Item
                {
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Category = category
                };

                items.Add(item);
                sb.AppendLine(string.Format(SuccessMessage, item.Name));

            }

            context.Items.AddRange(items);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(OrderDto[]), new XmlRootAttribute("Orders"));
            var deserializedOrders = (OrderDto[])serializer.Deserialize(new StringReader(xmlString));

            List<OrderItem> orderItems = new List<OrderItem>();
            List<Order> orders = new List<Order>();

            foreach (var orderDto in deserializedOrders)
            {
                bool isValidItem = true;

                if (!IsValid(orderDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                foreach (var itemDto in orderDto.OrderItems)
                {
                    if (!IsValid(itemDto))
                    {
                        sb.AppendLine(FailureMessage);
                        isValidItem = false;
                        break;
                    }
                }

                if (!isValidItem)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var employee = context.Employees.FirstOrDefault(x => x.Name == orderDto.Employee);

                if (employee == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var areValidItems = AreValidItems(context, orderDto.OrderItems);

                if (!areValidItems)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var date = DateTime.ParseExact(orderDto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var orderType = Enum.Parse<OrderType>(orderDto.Type);

                var order = new Order
                {
                    Customer = orderDto.Customer,
                    Employee = employee,
                    DateTime = date,
                    Type = orderType
                };

                orders.Add(order);

                foreach (var itemDto in orderDto.OrderItems)
                {
                    var item = context.Items.FirstOrDefault(x => x.Name == itemDto.Name);

                    var orderItem = new OrderItem
                    {
                        Order = order,
                        Item = item,
                        Quantity = itemDto.Quantity
                    };

                    orderItems.Add(orderItem);
                }

                sb.AppendLine($"Order for {orderDto.Customer} on {date.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} added");
            }

            context.Orders.AddRange(orders);
            context.SaveChanges();

            context.OrderItems.AddRange(orderItems);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool AreValidItems(FastFoodDbContext context, OrderItemDto[] orderItems)
        {
            foreach (var item in orderItems)
            {
                var itemExists = context.Items.Any(x => x.Name == item.Name);

                if (!itemExists)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResult, true);
        }

      

    }
}