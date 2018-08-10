using AutoMapper;
using ProductShop.App.DTOs;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using ProductShop.Data;
using System.Linq;
using ProductShop.App.DTOs.Export;
using System.Text;

namespace ProductShop.App
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            UsersAndProducts(context);



        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResult, true);
        }

        public static void AddProducts(ProductShopContext context, IMapper mapper)
        {
            var xmlString = File.ReadAllText("../../../XML/products.xml");

            var selializer = new XmlSerializer(typeof(ProductDto[]), new XmlRootAttribute("products"));
            var deselializedProducts = (ProductDto[])selializer.Deserialize(new StringReader(xmlString));

            List<Product> products = new List<Product>();

            int counter = 1;
            foreach (var productDto in deselializedProducts)
            {
                if (!IsValid(productDto))
                {
                    continue;
                }


                var product = mapper.Map<Product>(productDto);
                var buyerId = new Random().Next(1, 30);
                var sellerId = new Random().Next(31, 56);

                product.BuyerId = buyerId;
                product.SellerId = sellerId;

                if (counter == 4)
                {
                    product.BuyerId = null;
                    counter = 0;
                }

                products.Add(product);

                counter++;
            }


            context.Products.AddRange(products);
            context.SaveChanges();
        }


        public static void AddCategories(ProductShopContext context, IMapper mapper)
        {
            var xmlString = File.ReadAllText("../../../XML/categories.xml");

            var selializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("categories"));
            var deselializedCategories = (CategoryDto[])selializer.Deserialize(new StringReader(xmlString));

            List<Category> categories = new List<Category>();


            foreach (var categoryDto in deselializedCategories)
            {
                if (!IsValid(categoryDto))
                {
                    continue;
                }


                var category = mapper.Map<Category>(categoryDto);
                categories.Add(category);


            }


            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        public static void AddCategoryProducts(ProductShopContext context)
        {
            var categoriesProducts = new List<CategoryProducts>();

            for (int productId = 6; productId <= 203; productId++)
            {
                var categoryId = new Random().Next(1, 12);

                var categoryProduct = new CategoryProducts
                {
                    CategoryId = categoryId,
                    ProductId = productId
                };

                categoriesProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();
        }

        public static void ProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                                   .Where(p => p.Price >= 1000 && p.Price <= 2000 && p.Buyer != null)
                                   .OrderByDescending(p => p.Price)
                                   .Select(s => new ProductsInRangeDto
                                   {
                                       Price = s.Price,
                                       Name = s.Name,
                                       Buyer = s.Buyer.FirstName + " " + s.Buyer.LastName ?? s.Buyer.LastName
                                   }).ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ProductsInRangeDto[]), new XmlRootAttribute("products"));
            serializer.Serialize(new StringWriter(sb), products);

            File.WriteAllText("../../../XML/products-in-range.xml", sb.ToString());
        }


        public static void SoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x=> x.SelledProducts.Count >= 1)
                .OrderBy(x=> x.LastName)
                .ThenBy(x=> x.FirstName)
                                   .Select(s => new UserSoldProducts
                                   {
                                       FirstName = s.FirstName,
                                       LastName = s.LastName,
                                       SoldProduct = s.SelledProducts.Select(x => new SoldProduct
                                       {
                                           Name = x.Name,
                                           Price = x.Price
                                       })
                                       .ToArray()
                                   }).ToArray();



            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(UserSoldProducts[]), new XmlRootAttribute("users"));
            serializer.Serialize(new StringWriter(sb), users);

            File.WriteAllText("../../../XML/users-sold-products.xml", sb.ToString());
        }

        public static void CategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoriesByProductCountExportDto
                {
                    Name = c.Name,
                    ProductCount = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts
                    .Where(cp => cp.Product.Buyer != null)
                    .Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.ProductCount)
                .ToArray();

            var serializer = new XmlSerializer(typeof(CategoriesByProductCountExportDto[]), new XmlRootAttribute("categories"));

           

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), categories);

            var result = sb.ToString();

            File.WriteAllText("../../../XML/categories-by-products.xml", result);
        }

        public static void UsersAndProducts(ProductShopContext context)
        {
            var usersQuery = context.Users.Where(uc => uc.SelledProducts.Count > 0);

            var users = new UserExportDto
            {
                Count = usersQuery.Count(),
                Users = usersQuery
                    .Select(uc => new UserAndProductsDto
                    {
                        FirstName = uc.FirstName,
                        LastName = uc.LastName,
                        Age = uc.Age,
                        SoldProducts = new SoldProductsExportDto
                        {
                            Count = uc.SelledProducts.Count,
                            Products = uc.SelledProducts
                                .Select(sp => new ProductByUserExportDto
                                {
                                    Name = sp.Name,
                                    Price = sp.Price
                                })
                                .ToArray()
                        }
                    })
                    .ToArray(),
            };

            var serializer = new XmlSerializer(typeof(UserExportDto));
           

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), users);

            var result = sb.ToString();

            File.WriteAllText("../../../XML/users-and-products.xml", result);
        }
    }
}
