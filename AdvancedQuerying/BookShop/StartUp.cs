namespace BookShop
{
    using BookShop.Data;
    using BookShop.Initializer;
    using BookShop.Models;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {

                Console.WriteLine(RemoveBooks(new BookShopContext()));
            }
        }
        

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            var books = context.Books
                        .Where(x => x.AgeRestriction == ageRestriction)
                        .Select(x => x.Title)
                        .OrderBy(t=> t)
                        .ToList();
            return String.Join(Environment.NewLine,books);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                               .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                               .OrderBy(x => x.BookId)
                               .Select(x=>x.Title)
                               .ToList();
            return String.Join(Environment.NewLine,books);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();

            context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList()
                .ForEach(t => sb.AppendLine($"{t.Title} - ${t.Price:f2}"));

            return sb.ToString();
        }

        public static string GetBooksNotRealeasedIn(BookShopContext context, int year)
        {
            var sb = new StringBuilder();

            context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b=> b.BookId)
                .Select(b => b.Title)                             
                .ToList()
                .ForEach(t => sb.AppendLine(t));

            return sb.ToString();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower()).ToArray();

            var booksTitles = context.Books
                .Where(b => b.BookCategories
                    .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, booksTitles);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(b => b.ReleaseDate < releaseDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new { b.Title, b.EditionType, b.Price })
                .ToList();

            return String.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authorNames = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => $"{a.FirstName} {a.LastName}")
                .OrderBy(a => a)
                .ToList();

            return String.Join(Environment.NewLine, authorNames);

        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var booksTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, booksTitles);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksWithAuthor = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new { b.Title, b.Author.FirstName, b.Author.LastName })
                .ToList();

            return String.Join(Environment.NewLine, booksWithAuthor.Select(b => $"{b.Title} ({b.FirstName} {b.LastName})"));
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var copiesByAuthor = context.Authors
                .Select(a => new
                {
                    Name = $"{a.FirstName} {a.LastName}",
                    Copies = a.Books.Select(b => b.Copies).Sum()
                })
                .OrderByDescending(a => a.Copies)
                .ToList();

            return String.Join(Environment.NewLine, copiesByAuthor.Select(c => $"{c.Name} - {c.Copies}"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
                                .Where(b => b.Title.Length > lengthCheck)
                                .Count();

            return booksCount;
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitsByCategory = context.Categories
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks.Select(cb => cb.Book.Copies * cb.Book.Price).Sum()
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.Name)
                .ToList();

            return String.Join(Environment.NewLine, profitsByCategory.Select(p => $"{p.Name} ${p.Profit:f2}"));
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoriesWithBooks = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks
                        .Select(cb => cb.Book)
                        .OrderByDescending(b => b.ReleaseDate)
                        .Take(3)
                })
                .ToList();

            return String.Join(Environment.NewLine,
                categoriesWithBooks
                    .Select(c => $"--{c.Name}{Environment.NewLine}{String.Join(Environment.NewLine, c.Books.Select(b => $"{b.Title} ({b.ReleaseDate.Value.Year})"))}"));
        }

        public static void IncreasePrices(BookShopContext context)
        {
            context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToList()
                .ForEach(b => b.Price += 5);

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksForDelete = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.RemoveRange(booksForDelete);
            context.SaveChanges();

            //return context.SaveChanges(); not working in judge

            return booksForDelete.Count;
        }
    }
}
