using EfCoreBP.ApiService.Data;
using EfCoreBP.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBP.ApiService.Services;

public class DatabaseSeeder(BookStoreContext context)
{
    public async Task SeedAsync()
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Genres.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Genres
        var genres = new List<Genre>
        {
            new() { Name = "Fiction", Description = "Literary works of fiction including novels and short stories" },
            new() { Name = "Science Fiction", Description = "Speculative fiction dealing with futuristic concepts" },
            new() { Name = "Mystery", Description = "Fiction dealing with the solution of a crime or puzzle" },
            new() { Name = "Romance", Description = "Fiction dealing primarily with love stories" },
            new() { Name = "Fantasy", Description = "Fiction set in imaginary worlds with magical elements" },
            new() { Name = "Non-Fiction", Description = "Factual books including biographies, history, and self-help" },
            new() { Name = "Biography", Description = "Life stories of real people" },
            new() { Name = "History", Description = "Books about historical events and periods" }
        };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();

        // Seed Authors
        var authors = new List<Author>
        {
            new()
            {
                FirstName = "George",
                LastName = "Orwell",
                Email = "george.orwell@example.com",
                DateOfBirth = new DateTime(1903, 6, 25),
                Biography = "British author and journalist known for his dystopian novels"
            },
            new()
            {
                FirstName = "Jane",
                LastName = "Austen",
                Email = "jane.austen@example.com",
                DateOfBirth = new DateTime(1775, 12, 16),
                Biography = "English novelist known for her social commentary and wit"
            },
            new()
            {
                FirstName = "Isaac",
                LastName = "Asimov",
                Email = "isaac.asimov@example.com",
                DateOfBirth = new DateTime(1920, 1, 2),
                Biography = "American writer and professor of biochemistry, famous for science fiction"
            },
            new()
            {
                FirstName = "Agatha",
                LastName = "Christie",
                Email = "agatha.christie@example.com",
                DateOfBirth = new DateTime(1890, 9, 15),
                Biography = "English writer known for detective novels featuring Hercule Poirot"
            },
            new()
            {
                FirstName = "J.R.R.",
                LastName = "Tolkien",
                Email = "jrr.tolkien@example.com",
                DateOfBirth = new DateTime(1892, 1, 3),
                Biography = "English writer and philologist, best known for The Hobbit and Lord of the Rings"
            }
        };

        await context.Authors.AddRangeAsync(authors);
        await context.SaveChangesAsync();

        // Get genres for foreign keys
        var fictionGenre = await context.Genres.FirstAsync(g => g.Name == "Fiction");
        var sciFiGenre = await context.Genres.FirstAsync(g => g.Name == "Science Fiction");
        var mysteryGenre = await context.Genres.FirstAsync(g => g.Name == "Mystery");
        var fantasyGenre = await context.Genres.FirstAsync(g => g.Name == "Fantasy");

        // Seed Books
        var books = new List<Book>
        {
            new()
            {
                Title = "1984",
                ISBN = "978-0451524935",
                PublishedDate = new DateTime(1949, 6, 8),
                PageCount = 328,
                Price = 12.99m,
                Description = "A dystopian social science fiction novel about totalitarian control",
                GenreId = fictionGenre.Id
            },
            new()
            {
                Title = "Animal Farm",
                ISBN = "978-0451526342",
                PublishedDate = new DateTime(1945, 8, 17),
                PageCount = 112,
                Price = 9.99m,
                Description = "An allegorical novella about farm animals who rebel against their owner",
                GenreId = fictionGenre.Id
            },
            new()
            {
                Title = "Pride and Prejudice",
                ISBN = "978-0141439518",
                PublishedDate = new DateTime(1813, 1, 28),
                PageCount = 432,
                Price = 11.99m,
                Description = "A romantic novel about manners, upbringing, morality, and marriage",
                GenreId = fictionGenre.Id
            },
            new()
            {
                Title = "Foundation",
                ISBN = "978-0553293357",
                PublishedDate = new DateTime(1951, 5, 1),
                PageCount = 244,
                Price = 14.99m,
                Description = "A science fiction novel about the fall and rise of galactic civilization",
                GenreId = sciFiGenre.Id
            },
            new()
            {
                Title = "Murder on the Orient Express",
                ISBN = "978-0062693662",
                PublishedDate = new DateTime(1934, 1, 1),
                PageCount = 256,
                Price = 13.99m,
                Description = "A detective novel featuring Hercule Poirot",
                GenreId = mysteryGenre.Id
            },
            new()
            {
                Title = "The Hobbit",
                ISBN = "978-0547928227",
                PublishedDate = new DateTime(1937, 9, 21),
                PageCount = 310,
                Price = 15.99m,
                Description = "A fantasy novel about the adventures of Bilbo Baggins",
                GenreId = fantasyGenre.Id
            }
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        // Get authors and books for relationships
        var orwell = await context.Authors.FirstAsync(a => a.LastName == "Orwell");
        var austen = await context.Authors.FirstAsync(a => a.LastName == "Austen");
        var asimov = await context.Authors.FirstAsync(a => a.LastName == "Asimov");
        var christie = await context.Authors.FirstAsync(a => a.LastName == "Christie");
        var tolkien = await context.Authors.FirstAsync(a => a.LastName == "Tolkien");

        var book1984 = await context.Books.FirstAsync(b => b.Title == "1984");
        var animalFarm = await context.Books.FirstAsync(b => b.Title == "Animal Farm");
        var prideAndPrejudice = await context.Books.FirstAsync(b => b.Title == "Pride and Prejudice");
        var foundation = await context.Books.FirstAsync(b => b.Title == "Foundation");
        var murderOnOrient = await context.Books.FirstAsync(b => b.Title == "Murder on the Orient Express");
        var hobbit = await context.Books.FirstAsync(b => b.Title == "The Hobbit");

        // Seed BookAuthors (many-to-many relationships)
        var bookAuthors = new List<BookAuthor>
        {
            new()
            {
                BookId = book1984.Id,
                AuthorId = orwell.Id,
                IsPrimaryAuthor = true,
                ContributionDate = DateTime.Now.AddDays(-30)
            },
            new()
            {
                BookId = animalFarm.Id,
                AuthorId = orwell.Id,
                IsPrimaryAuthor = true,
                ContributionDate = DateTime.Now.AddDays(-25)
            },
            new()
            {
                BookId = prideAndPrejudice.Id,
                AuthorId = austen.Id,
                IsPrimaryAuthor = true,
                ContributionDate = DateTime.Now.AddDays(-20)
            },
            new()
            {
                BookId = foundation.Id,
                AuthorId = asimov.Id,
                IsPrimaryAuthor = true,
                ContributionDate = DateTime.Now.AddDays(-15)
            },
            new()
            {
                BookId = murderOnOrient.Id,
                AuthorId = christie.Id,
                IsPrimaryAuthor = true,
                ContributionDate = DateTime.Now.AddDays(-10)
            },
            new()
            {
                BookId = hobbit.Id,
                AuthorId = tolkien.Id,
                IsPrimaryAuthor = true,
                ContributionDate = DateTime.Now.AddDays(-5)
            }
        };

        await context.BookAuthors.AddRangeAsync(bookAuthors);
        await context.SaveChangesAsync();

        Console.WriteLine("Database seeded successfully!");
    }

}
