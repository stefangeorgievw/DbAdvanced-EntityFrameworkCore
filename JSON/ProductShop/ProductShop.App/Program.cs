using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ProductShop.App
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ProductShopContext())
            {
                CategoriesByProductCount(context);
            }
        }


        public static void ProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                                  .Where(p => p.Price >= 500 && p.Price <= 1000)
                                  .OrderBy(p => p.Price)
                                  .Select(s => new
                                  {
                                      name = s.Name,
                                      price = s.Price,
                                      seller = s.Seller.FirstName + " " + s.Seller.LastName ?? s.Seller.LastName
                                  }).ToArray();

            var jsonString = JsonConvert.SerializeObject(products, Formatting.Indented);

            File.WriteAllText("../../../JSON/products-in-range.json", jsonString);
        }

        public static void CategoriesByProductCount(ProductShopContext context)
        {
            var categories = context.Categories
                    .Select(c => new
                    {
                        category = c.Name,
                        productsCount = c.CategoryProducts.Count,
                        averagePrice = c.CategoryProducts.Average(p => p.Product.Price),
                        totalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                    })
                            
                    .OrderByDescending(o => o.productsCount)
                    .ToList();

            var jsonString = JsonConvert.SerializeObject(categories, Formatting.Indented);

            File.WriteAllText("../../../JSON/categories-by-products.json", jsonString);

        }

        public static void SuccessfullySoldProducts(ProductShopContext context)
        {
            var users = context.Users
                   .Where(u => u.SelledProducts.Count > 1 && u.SelledProducts.Any(s => s.Buyer != null))
                   .Select(u => new
                   {
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       SoldProducts = u.SelledProducts
                                                     .Where(s => s.Buyer != null)
                                                    .Select(p => new
                                                    {
                                                        Name = p.Name,
                                                        Price = p.Price,
                                                        BuyerFirstName = p.Buyer.FirstName,
                                                        BuyerLastName = p.Buyer.LastName
                                                    })
                   })
                   .OrderBy(o => o.LastName)
                   .ThenBy(o => o.FirstName)
                   .ToList();


            var jsonString = JsonConvert.SerializeObject(users, Formatting.Indented);

            File.WriteAllText("../../../JSON/users-sold-products.json", jsonString);
        }

        public static void UsersAndProducts(ProductShopContext context)
        {
            var users = new
            {
                usersCount = context.Users.Count(),
                users = context.Users
                    .Where(u => u.SelledProducts.Count > 1)
                    .OrderByDescending(o => o.SelledProducts.Count)
                    .ThenBy(o => o.LastName)
                    .Select(z => new
                    {
                        FirstName = z.FirstName,
                        LastName = z.LastName,
                        Age = z.Age,
                        SoldProducts = new { Count = z.SelledProducts.Count(), Products = z.SelledProducts.Select(p => new { Name = p.Name, Price = p.Price }) }
                    })
                    .ToList()
            };


            var jsonString = JsonConvert.SerializeObject(users, Formatting.Indented);

            File.WriteAllText("../../../JSON/users-and-products.json", jsonString);

        }

        public static void ImportUsers(ProductShopContext context)
        {
            var jsonString = File.ReadAllText("../../../JSON/users.json");

            var deseliarisedUsers = JsonConvert.DeserializeObject<User[]>(jsonString);

            var users = new List<User>();

            foreach (var user in deseliarisedUsers)
            {
                if (!IsValid(user))
                {
                    continue;
                }
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        public static void ImportProducts(ProductShopContext context)
        {
            var jsonString = File.ReadAllText("../../../JSON/products.json");

            var deseliarisedProducts = JsonConvert.DeserializeObject<Product[]>(jsonString);

            var products = new List<Product>();

            foreach (var product in deseliarisedProducts)
            {
                if (!IsValid(product))
                {
                    continue;
                }
                var sellerId = new Random().Next(1, 35);
                var buyerId = new Random().Next(35, 57);

                product.SellerId = sellerId;
                product.BuyerId = buyerId;
                var random = new Random().Next(1, 4);
                if (random == 3)
                {
                    product.BuyerId = null;
                }
                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        public static void ImportCategories(ProductShopContext context)
        {
            var jsonString = File.ReadAllText("../../../JSON/categories.json");

            var deseliarisedCategories = JsonConvert.DeserializeObject<Category[]>(jsonString);

            var categories = new List<Category>();

            foreach (var category in deseliarisedCategories)
            {
                if (!IsValid(category))
                {
                    continue;
                }
                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        public static void GenerateCategoryProducts(ProductShopContext context)
        {
            var categoryProducts = new List<CategoryProducts>();

            for (int productId = 1; productId <= 200; productId++)
            {
                var categoryId = new Random().Next(1, 12);

                var categoryProduct = new CategoryProducts
                {
                    CategoryId = categoryId,
                    ProductId = productId
                };

                categoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResult, true);
        }
    }
}
