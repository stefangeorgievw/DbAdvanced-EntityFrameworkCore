using AutoMapper;
using CarDealer.App.Dtos.Export;
using CarDealer.App.Dtos.Import;
using CarDealer.Data;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer.App
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var context = new CarDealerContext())
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.AddProfile<CarDealerProfile>();
                });

                SalesWithAppliedDiscount(context);
            }
            

        }

        public static void ImportSuppliers(CarDealerContext context, IMapper mapper)
        {
            var xmlString = File.ReadAllText("../../../Xml/suppliers.xml");

            var serializer = new XmlSerializer(typeof(SupplierDto[]), new XmlRootAttribute("suppliers"));

            var supplierDtos = (SupplierDto[])serializer.Deserialize(new StringReader(xmlString));

            var suppliers = new List<Supplier>();

            foreach (var supplierDto in supplierDtos)
            {
                if (!IsValid(supplierDto))
                {
                    continue;
                }
                var supplier = mapper.Map<Supplier>(supplierDto);
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

           
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResult, true);
        }

        public static void ImportParts(CarDealerContext context, IMapper mapper)
        {
            var xmlString = File.ReadAllText("../../../Xml/parts.xml");

            var serializer = new XmlSerializer(typeof(PartDto[]), new XmlRootAttribute("parts"));

            var partDtos = (PartDto[])serializer.Deserialize(new StringReader(xmlString));

            var parts = new List<Part>();

            foreach (var partDto in partDtos)
            {
                if (!IsValid(partDto))
                {
                    continue;
                }
                var part = mapper.Map<Part>(partDto);
                parts.Add(part);
                var randomSupplier = new Random().Next(1, 32);
                part.Supplier_Id = randomSupplier;
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();


        }

        public static void ImportCars(CarDealerContext context, IMapper mapper)
        {
            var xmlString = File.ReadAllText("../../../Xml/cars.xml");

            var serializer = new XmlSerializer(typeof(CarDto[]), new XmlRootAttribute("cars"));

            var carsDtos = (CarDto[])serializer.Deserialize(new StringReader(xmlString));

            ICollection<Car> cars = new List<Car>();

            ICollection<Part> parts = context.Parts.ToList();

            foreach (var carDto in carsDtos)
            {
                if (!IsValid(carDto))
                {
                    continue;
                }
                var car = mapper.Map<Car>(carDto);
                cars.Add(car);
               

            }
            cars = AddPartsToCars(parts, cars);
            

             context.Cars.AddRange(cars);
            context.SaveChanges();


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

        public static void ImportCustomers(CarDealerContext context, IMapper mapper)
        {
            var xmlString = File.ReadAllText("../../../Xml/customers.xml");

            var serializer = new XmlSerializer(typeof(CustomertDto[]), new XmlRootAttribute("customers"));

            var customerDtos = (CustomertDto[])serializer.Deserialize(new StringReader(xmlString));

            var customers = new List<Customer>();

            foreach (var customerDto in customerDtos)
            {
                if (!IsValid(customerDto))
                {
                    continue;
                }
                var customer = mapper.Map<Customer>(customerDto);
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();


        }

        private static void  GenerateSales(ICollection<Car> cars, ICollection<Customer> customers, CarDealerContext context)
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

        public static void CarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new CarWithDistanceExportDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    Distance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .ToArray();

            var serializer = new XmlSerializer(typeof(CarWithDistanceExportDto[]), new XmlRootAttribute("cars"));
          

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), cars);

            var result = sb.ToString();
            File.WriteAllText("../../../Xml/carsExport.xml", result);
            
        }

        public static void CarsFromMakeFerrari(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Ferrari")
                .Select(c => new CarsFromMakeFerrariDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    Distance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.Distance)
                .ToArray();

            var serializer = new XmlSerializer(typeof(CarsFromMakeFerrariDto[]), new XmlRootAttribute("cars"));


            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), cars);

            var result = sb.ToString();
            File.WriteAllText("../../../Xml/carsFromMakeFerrari.xml", result);

        }

        public static void LocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierExportDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Count = s.Parts.Count()
                })
                .OrderBy(s=> s.Name)
                .ToArray();

            var serializer = new XmlSerializer(typeof(LocalSupplierExportDto[]), new XmlRootAttribute("suppliers"));

          

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), suppliers);

            var result = sb.ToString();

            File.WriteAllText("../../../Xml/localSuppliers.xml", result);
        }

        public static void CarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                              .Select(c => new CarsAndTheirPartsExportDto
                              {
                                  Make = c.Make,
                                  Model = c.Model,
                                  Distance = c.TravelledDistance,
                                  Parts = c.PartCars.Select(p=> new PartsForCarExportDto
                                  {
                                      Name = p.Part.Name,
                                      Price = p.Part.Price
                                  }).ToArray()
                              }).ToArray();

            var serializer = new XmlSerializer(typeof(CarsAndTheirPartsExportDto[]), new XmlRootAttribute("cars"));



            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), cars);

            var result = sb.ToString();

            File.WriteAllText("../../../Xml/cars-and-parts.xml", result);
        }

        public static void TotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new TotalSalesByCustomerExportDto
                {
                    FullName = c.Name,
                    CatsCount = c.Sales.Count(),
                    PriceAndDiscounts = c.Sales
                        .Select(s => new CarPriceAndDiscountExportDto
                        {
                            TotalPrice = s.Car
                                        .PartCars
                                        .Sum(p =>
                                            p.Part.Price * p.Part.Quantity),
                            Discount = (double)s.Discount / 100
                        })
                        .ToArray()
                })
                .OrderByDescending(c => c.MoneySpent)
                .ThenByDescending(c => c.CatsCount)
                .ToArray();

            var serializer = new XmlSerializer(typeof(TotalSalesByCustomerExportDto[]), new XmlRootAttribute("customers"));
          

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), customers);

            var result = sb.ToString();

            File.WriteAllText("../../../Xml/customers-total-sells.xml", result);

     
        }

        public static void SalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SalesWithAppliedDiscountExportDto
                {
                    Car = new CarAndDistanceAttributeExportDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        Distance = s.Car.TravelledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Dicount = ((decimal)s.Discount / 100).ToString(),
                    Price = s.Car.PartCars.Sum(p => p.Part.Price * p.Part.Quantity),
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(SalesWithAppliedDiscountExportDto[]), new XmlRootAttribute("sales"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), sales, namespaces);

            var result = sb.ToString();

            File.WriteAllText("../../../Xml/salles-discounts.xml", result);


        }
    }
}
