using Microsoft.EntityFrameworkCore;
using P01_BillsPaymentSystem.Data;
using P01_BillsPaymentSystem.Data.Models;
using P01_BillsPaymentSystem.Data.Models.Enums;
using System;
using System.Globalization;
using System.Linq;

namespace P01_BillsPaymentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
            {
                context.Database.EnsureDeleted();

                context.Database.EnsureCreated();

                Seed(context);


            }
        }

        static void Seed(BillsPaymentSystemContext db)
        {
            var users = new[]
            {
                new User
                {
                    FirstName = "Pesho",
                    LastName = "Ivanov",
                    Email = "pesho@abv.bg",
                    Password = "123"
                },

                new User
                {
                    FirstName = "Gosho",
                    LastName = "Petrov",
                    Email = "gosho@gmail.com",
                    Password = "234"
                },

                new User
                {
                    FirstName = "Stamat",
                    LastName = "Todorov",
                    Email = "stamat@yahoo.com",
                    Password = "345"
                },

                new User
                {
                    FirstName = "Toshko",
                    LastName = "Vladimirov",
                    Email = "toshko@bnr.bg",
                    Password = "456"
                }
            };

            db.Users.AddRange(users);

            var creditCards = new[]
            {
                new CreditCard(new DateTime(2018, 6, 20), 15000.00m, 1500.00m),
                new CreditCard(new DateTime(2018, 6, 25), 20000m, 1800m),
                new CreditCard(new DateTime(2019, 7, 4), 15000m, 14000m),
                new CreditCard(new DateTime(2019, 2, 5), 16000m, 4500m)
            };

            db.CreditCards.AddRange(creditCards);

            var bankAccounts = new[]
            {
                new BankAccount(2455m, "SG Expresbank", "TGBHJKL"),
                new BankAccount(12000m, "Investbank", "TBGINKFL"),
                new BankAccount(14000m, "DSK", "TBGDSK"),
                new BankAccount(8500m, "Raiffensen bank", "TBGFRF")
            };

            db.BankAccounts.AddRange(bankAccounts);

            var paymentMethods = new[]
            {
                new PaymentMethod
                {
                    User = users[0],
                    Type = Data.Models.Enums.Type.BankAccount,
                    BankAccount = bankAccounts[0]
                },

                new PaymentMethod
                {
                    User = users[0],
                    Type = Data.Models.Enums.Type.BankAccount,
                    BankAccount = bankAccounts[1]
                },

                new PaymentMethod
                {
                    User = users[0],
                    Type = Data.Models.Enums.Type.CreditCard,
                    CreditCard = creditCards[0]
                },

                new PaymentMethod
                {
                    User = users[1],
                    Type = Data.Models.Enums.Type.CreditCard,
                    CreditCard = creditCards[1]
                },

                new PaymentMethod
                {
                    User = users[2],
                    Type = Data.Models.Enums.Type.BankAccount,
                    BankAccount = bankAccounts[2]
                },

                new PaymentMethod
                {
                    User = users[2],
                    Type = Data.Models.Enums.Type.CreditCard,
                    CreditCard = creditCards[2]
                },

                new PaymentMethod
                {
                    User = users[2],
                    Type = Data.Models.Enums.Type.CreditCard,
                    CreditCard = creditCards[3]
                },

                new PaymentMethod
                {
                    User = users[3],
                    Type = Data.Models.Enums.Type.BankAccount,
                    BankAccount = bankAccounts[3]
                }
            };

            db.PaymentMethods.AddRange(paymentMethods);

            db.SaveChanges();
        }

        public static void UserDetails(int userId, BillsPaymentSystemContext db)
        {
            using (db = new BillsPaymentSystemContext())
            {
                var user = db.Users
                .Where(x => x.UserId == userId)
                .Select(x => new
                {
                    Name = $"{x.FirstName} {x.LastName}",
                    BankAccounts = x.PaymentMethods
                        .Where(a => a.Type == Data.Models.Enums.Type.BankAccount)
                        .Select(b => b.BankAccount).ToList(),
                    CreditCards = x.PaymentMethods
                        .Where(a => a.Type == Data.Models.Enums.Type.CreditCard)
                        .Select(n => n.CreditCard).ToList()
                }).FirstOrDefault();

                if (user == null)
                {
                    Console.WriteLine("There is no user with that Id");
                    return;
                }
                Console.WriteLine($"User: {user.Name}");

                if (user.BankAccounts.Any())
                {
                    Console.WriteLine("Bank Accounts:");
                    foreach (var item in user.BankAccounts)
                    {
                        Console.WriteLine($"-- ID: {item.BankAccountId}");
                        Console.WriteLine($"--- Balance: {item.Balance:f2}");
                        Console.WriteLine($"--- Bank: {item.BankName}");
                        Console.WriteLine($"--- SWIFT: {item.SwiftCode}");
                    }
                }

                if (!user.CreditCards.Any()) return;
                {
                    Console.WriteLine("Credit Cards:");
                    foreach (var item in user.CreditCards)
                    {
                        Console.WriteLine($"-- ID: {item.CreditCardId}");
                        Console.WriteLine($"--- Limit: {item.Limit:f2}");
                        Console.WriteLine($"--- Money Owed: {item.MoneyOwed:f2}");
                        Console.WriteLine($"--- Limit Left: {item.LimitLeft}");
                        Console.WriteLine($"--- Expiration Date: {item.ExpirationDate.ToString(@"yyyy/MM",CultureInfo.InvariantCulture)}");
                    }
                }
            }

        }

        private static void PayBills(int userId, decimal amount, BillsPaymentSystemContext db)
        {
            using (db = new BillsPaymentSystemContext())
            {
                var user = db.Users
                    .Include(x => x.PaymentMethods)
                    .FirstOrDefault(x => x.UserId == userId);

                var accounts = db.PaymentMethods
                        .Include(pm => pm.BankAccount)
                        .Where(pm => pm.UserId == userId && pm.Type == Data.Models.Enums.Type.BankAccount)
                        .Select(pm => pm.BankAccount)
                        .ToList();

                var cards = db.PaymentMethods
                    .Include(pm => pm.CreditCard)
                    .Where(pm => pm.UserId == userId && pm.Type == Data.Models.Enums.Type.CreditCard)
                    .Select(pm => pm.CreditCard)
                    .ToList();

                try
                {
                    foreach (var ba in accounts)
                    {
                        ba.Withdraw(amount);
                    }
                    foreach (var cc in cards)
                    {
                        cc.Withdraw(amount);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
