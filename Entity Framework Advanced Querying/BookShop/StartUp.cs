using System;
using System.Globalization;
using System.Text;
using BookShop.Models;
using BookShop.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            
            //Console.WriteLine(GetBooksByAgeRestriction(db, input));
            //Console.WriteLine(GetGoldenBooks(db));
            //Console.WriteLine(GetBooksByPrice(db));
            //Console.WriteLine(GetBooksNotReleasedIn(db, input));
            //int input = int.Parse(Console.ReadLine());
            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestriction))
            {
                Console.WriteLine($"{command} is not a valid age restriction");
            }

            var books = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.EditionType == EditionType.Gold &&
                            b.Copies < 5000)
                .Select(b => new
                {
                    b.Title,
                    b.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();
                
            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var b in books)
            {
                result.AppendLine($"{b.Title} - ${b.Price:f2}");
            }

            return result.ToString().Trim();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    b.Title,
                    b.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var books = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var myDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < myDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var b in books)
            {
                result.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");
            }

            return result.ToString().Trim();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var books = context.Authors
                .Where(b => b.FirstName.Substring(b.FirstName.Length - input.Length, input.Length) ==
                            input)
                .Select(b => new
                {
                    AuthorName = b.FirstName + " " + b.LastName
                })
                .OrderBy(b => b.AuthorName)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.AuthorName));
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string[] bookTitlesContaining = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(title => title)
                .ToArray();

            return String.Join(Environment.NewLine, bookTitlesContaining);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    AuthorName = b.Author.FirstName + ' ' + b.Author.LastName
                })
                .OrderBy(b => b.BookId)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var b in books)
            {
                result.AppendLine($"{b.Title} ({b.AuthorName})");
            }

            return result.ToString().Trim();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return booksCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var numberOfBooks = context.Authors
                .Select(a => new
                {
                    Name = a.FirstName + ' ' + a.LastName,
                    CopiesCount = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.CopiesCount)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var a in numberOfBooks)
            {
                result.AppendLine($"{a.Name} - {a.CopiesCount}");
            }

            return result.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var booksByCategory = context.Categories
                .Select(b => new
                {
                    b.Name,
                    TotalPrice = b.CategoryBooks.Sum(bc => bc.Book.Copies * bc.Book.Price)
                })
                .OrderByDescending(b => b.TotalPrice)
                .ThenBy(b => b.Name)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var b in booksByCategory)
            {
                result.AppendLine($"{b.Name} ${b.TotalPrice:f2}");
            }

            return result.ToString().Trim();

        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var recentBooks = context.Categories
                .Select(c => new
                {
                    c.Name,
                    RecentBooks = c.CategoryBooks
                        .Select(b => new
                        {
                            BookTitle = b.Book.Title,
                            BookReleaseDate = b.Book.ReleaseDate
                        })
                        .OrderByDescending(b => b.BookReleaseDate)
                        .Take(3)
                        .ToList()
                })
                .OrderBy(c => c.Name)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var c in recentBooks)
            {
                result.AppendLine($"--{c.Name}");

                foreach (var b in c.RecentBooks)
                {
                    result.AppendLine($"{b.BookTitle} ({b.BookReleaseDate.Value.Year})");
                }
            }

            return result.ToString().Trim();

        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010);

            foreach (var b in books)
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.RemoveRange(booksToRemove);
            context.SaveChanges();

            return booksToRemove.Count();
        }
    }
}


