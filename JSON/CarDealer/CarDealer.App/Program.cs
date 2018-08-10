using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace CarDealer.App
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new CarDealerContext())
            {
                SalesWithDiscount(context);
            }
        }


        private static void OrderedCustomers(CarDealerContext context)
        {
           
                var customers = context.Customers
                    .OrderBy(c => c.BirthDate)
                    .ThenBy(c => c.IsYoungDriver)
                    .Select(s => new { Id = s.Id, Name = s.Name, BirthDate = s.BirthDate, IsYoungDriver = s.IsYoungDriver, Sales = s.Sales.Take(0) })
                    .ToList();

                

                var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

                File.WriteAllText(@"../../../json/ordered-customers.json", json);
            
        }

        private static void ToyotoCars(CarDealerContext context)
        {
           
                var cars = context.Cars
                    .Where(c => c.Make == "Toyota")
                    .OrderBy(m => m.Model)
                    .ThenByDescending(d => d.TravelledDistance)
                    .Select(s => new { Id = s.Id, Make = s.Make, Model = s.Model, TravelledDistance = s.TravelledDistance })
                    .ToList();

              

                var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

                File.WriteAllText(@"../../../json/toyota-cars.json", json);
            
        }

        public static void LocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                    .Where(s => s.IsImporter == false)
                    .Select(s => new { Id = s.Id, Name = s.Name, PartsCount = s.Parts.Count })
                    .ToList();


            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            File.WriteAllText("../../../json/local-suppliers.json", json);
        }

        private static void CarsWithParts(CarDealerContext context)
        {
            
                var carsAndParts = context.Cars
                    .Select(s => new
                    {
                        car = new { Make = s.Make, Model = s.Model, TravelledDistance = s.TravelledDistance },
                        parts = s.PartCars.Select(cp => new { Name = cp.Part.Name, Price = cp.Part.Price })
                    })
                    .ToList();

               

                var json = JsonConvert.SerializeObject(carsAndParts, Formatting.Indented);

                File.WriteAllText(@"../../../cars-and-parts.json", json);
            
        }

        private static void SalesByCustomer(CarDealerContext context)
        {
          
                var customers = context.Customers
                    .Where(c => c.Sales.Count >= 1)
                    .Select(s => new
                    {
                        fullName = s.Name,
                        boughtCars = s.Sales.Count,
                       spentMoney = (decimal)s.Sales.Sum(x=> x.Car.PartCars.Sum(p=> p.Part.Price))
                        
                    })
                    .OrderByDescending(o => o.spentMoney)
                    .ThenByDescending(o => o.boughtCars)
                    .ToList();

               

                var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

                File.WriteAllText(@"../../../json/customers-total-sales.json", json);
            
        }

        private static void SalesWithDiscount(CarDealerContext context)
        {
            
                var sales = context.Sales
                    .Select(s => new
                    {
                        car = new { Make = s.Car.Make, Model = s.Car.Model, TravelledDistance = s.Car.TravelledDistance },
                        CustomerName = s.Customer.Name,
                        Discount = s.Discount,
                        Price = s.Car.PartCars.Sum(p => p.Part.Price),
                        PriceWithDiscount = (s.Car.PartCars.Sum(p => p.Part.Price) - (s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount))
                    })
                    .ToList();

               

                var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

                File.WriteAllText(@"../../../json/sales-discounts.json", json);
            
        }


        public static void ImportSuppliers(CarDealerContext context)
        {
            var jsonString = File.ReadAllText("../../../json/suppliers.json");

            var desializedSuppliers = JsonConvert.DeserializeObject<Supplier[]>(jsonString);

            var suppliers = new List<Supplier>();

            foreach (var supplier in desializedSuppliers)
            {
                if (!IsValid(supplier))
                {
                    continue;
                }
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
        }

        public static void ImportParts(CarDealerContext context)
        {
            var jsonString = File.ReadAllText("../../../json/parts.json");

            var desialisedParts = JsonConvert.DeserializeObject<Part[]>(jsonString);

           

            var parts = new List<Part>();

            foreach (var part in desialisedParts)
            {
                if (!IsValid(part))
                {
                    continue;
                }

                parts.Add(part);
                var randomSupplier = new Random().Next(1, 32);
                part.Supplier_Id = randomSupplier;
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();


        }

        public static void ImportCars(CarDealerContext context)
        {
            var jsonString = File.ReadAllText("../../../json/cars.json");

            var desializedCars = JsonConvert.DeserializeObject<Car[]>(jsonString);

            ICollection<Car> cars = new List<Car>();

            ICollection<Part> parts = context.Parts.ToList();

            foreach (var car in desializedCars)
            {
                if (!IsValid(car))
                {
                    continue;
                }
                
                cars.Add(car);


            }
            cars = AddPartsToCars(parts, cars);


            context.Cars.AddRange(cars);
            context.SaveChanges();


        }

        public static void ImportCustomers(CarDealerContext context)
        {
            var jsonString = File.ReadAllText("../../../json/customers.json");

            var desializedCustomers = JsonConvert.DeserializeObject<Customer[]>(jsonString);

            var customers = new List<Customer>();

            foreach (var customer in desializedCustomers)
            {
                if (!IsValid(customer))
                {
                    continue;
                }
              
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();


        }

        private static void GenerateSales(ICollection<Car> cars, ICollection<Customer> customers, CarDealerContext context)
        {
            var sales = new Sale[cars.Count];

            Random random = new Random();

            for (int i = 0; i < sales.Length; i++)
            {
                Customer customer = customers.ElementAt(random.Next(customers.Count - 1));

                int discount = GetDiscount(customer.IsYoungDriver);

                sales[i] = new Sale
                {
                    Car = cars.ElementAt(i),
                    Customer = customer,
                    Discount = discount
                };
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();
        }

        private static int GetDiscount(bool isYoungDriver)
        {
            var discounts = new int[] { 0, 5, 10, 15, 20, 30, 40, 50 };

            Random random = new Random();

            int discount = discounts[random.Next(0, discounts.Length - 1)];
            if (isYoungDriver)
            {
                discount += 5;
            }

            return discount;
        }

        private static ICollection<Car> AddPartsToCars(ICollection<Part> parts, ICollection<Car> cars)
        {
            Random random = new Random();
            foreach (Car car in cars)
            {
                car.PartCars = GeneratePartCars(parts, random.Next(10, 20));
            }

            return cars;
        }

        private static ICollection<PartCar> GeneratePartCars(ICollection<Part> parts, int count)
        {
            var rangeOfParts = new List<Part>();
            Random random = new Random();
            while (rangeOfParts.Count < count)
            {
                rangeOfParts.Add(parts.ElementAt(random.Next(0, parts.Count - 1)));

                if (rangeOfParts.Count == count)
                {
                    rangeOfParts = rangeOfParts.Distinct().ToList();
                }
            }

            var partCars = new List<PartCar>();
            foreach (var part in rangeOfParts.Distinct())
            {
                partCars.Add(new PartCar
                {
                    Part = part
                });
            }

            return partCars;
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResult, true);
        }
    }
}
