using EfCoreBP.ApiService.Data;
using EfCoreBP.ApiService.DTOs;
using EfCoreBP.ApiService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBP.ApiService.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Books").WithTags(nameof(Book));

        static bool SomeLocalFunction(Book b) => b.Title.Length > 0;

        group.MapGet("/", async (BookStoreContext db) =>
        {
            return await db.Books
                .AsNoTracking()
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.PublishedDate,
                    b.PageCount,
                    b.Price,
                    b.Description,
                    b.GenreId,
                    Genre = b.Genre != null ? b.Genre.Name : null,
                    Authors = b.BookAuthors.Select(ba => new
                    {
                        AuthorId = ba.Author.Id,
                        AuthorName = ba.Author.FirstName + ' ' + ba.Author.LastName,
                        ba.IsPrimaryAuthor,
                        ba.ContributionDate
                    })
                })
                .ToListAsync();
        })
        .WithName("GetAllBooks")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Book>, NotFound>> (int id, BookStoreContext db) =>
        {
            return await db.Books.AsNoTracking()
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(model => model.Id == id)
                is Book model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetBookById")
        .WithOpenApi();

        group.MapGet("/untracked", async (BookStoreContext db) =>
        {
            // When we read data without need to update it, we should use AsNoTracking or anonymous types with .Select
            var untrackedBooks = await db.Books
                .AsNoTracking()
                .ToListAsync();

            var untrackedBookNames = await db.Books
                .Select(b => new { b.Title })
                .ToListAsync();
            // .Select to an anonymous type is also untracked, we don't need AsNoTracking here

            // Best practice is to use .Select with only data(columns) that we need from database = lower size of data transferred and queries are faster
            // For that we use DTOs (Data Transfer Objects)
            var bookNameDtos = await db.Books
                .Select(b => new BookTitleDto { Title = b.Title })
                .ToListAsync();
        })
        .WithName("Untracked")
        .WithOpenApi();

        group.MapGet("/tracked", async (BookStoreContext db) =>
        {
            // Tracked is all queries without AsNoTracking or .Select to anonymous types*
            // *Tracked object are only entities that are part of DbSet in DbContext
            var trackedBooks = await db.Books.ToListAsync();
            Console.WriteLine($"After loading books: {db.ChangeTracker.Entries().Count()} tracked entities");

            var trackedAuthors = await db.Authors.ToListAsync();
            Console.WriteLine($"After loading authors: {db.ChangeTracker.Entries().Count()} tracked entities");

            // we can check what is tracked in the context for debugging purposes
            var trackedEntities = db.ChangeTracker.Entries()
                .Select(e => new
                {
                    EntityType = e.Entity.GetType().Name,
                    State = e.State.ToString(),
                    e.Entity
                })
                .ToList();

            // Clear tracker to demonstrate untracked queries
            db.ChangeTracker.Clear();
            Console.WriteLine($"After clearing: {db.ChangeTracker.Entries().Count()} tracked entities");

            // 1. AsNoTracking() - explicitly untracked
            var untrackedBooks = await db.Books
                .AsNoTracking()
                .ToListAsync();
            Console.WriteLine($"After AsNoTracking() query: {db.ChangeTracker.Entries().Count()} tracked entities");

            // 2. Projection to primitive types - untracked
            var bookTitles = await db.Books
                .Select(b => b.Title)
                .ToListAsync();
            Console.WriteLine($"After primitive projection: {db.ChangeTracker.Entries().Count()} tracked entities");

            // 3. Projection to anonymous types - untracked
            var bookSummaries = await db.Books
                .Select(b => new { b.Title, b.Price })
                .ToListAsync();
            Console.WriteLine($"After anonymous projection: {db.ChangeTracker.Entries().Count()} tracked entities");

            // 4. Projection to DTOs - untracked
            var bookDtos = await db.Books
                .Select(b => new BookTitleDto { Title = b.Title })
                .ToListAsync();
            Console.WriteLine($"After DTO projection: {db.ChangeTracker.Entries().Count()} tracked entities");

            // Load a book for tracking
            var bookToUpdate = await db.Books
                .FirstAsync(b => b.Title.Contains("Hobbit"));

            Console.WriteLine($"Loaded book for update: {db.ChangeTracker.Entries().Count()} tracked entities");

            var bookEntry = db.Entry(bookToUpdate);

            var originalTitle = bookToUpdate.Title;
            bookToUpdate.Title = "The Hobbit - Updated Edition";

            // Show modified entities
            var modifiedEntities = db.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .Select(e => new
                {
                    EntityType = e.Entity.GetType().Name,
                    State = e.State.ToString()
                })
                .ToList();

            // why we need tracked entities?
            // Tracked entities are saved/updated in database when we call SaveChanges or SaveChangesAsync
            // It's fine for small numbers of entities that are read in memory and then updated
            await db.SaveChangesAsync();
            Console.WriteLine("Changes saved to database");

            // Reset for demo purposes
            bookToUpdate.Title = originalTitle;
            await db.SaveChangesAsync();

        })
        .WithName("Tracked")
        .WithOpenApi();

        group.MapGet("/update", async (BookStoreContext db) =>
        {
            // Untracked entity will not be collected in Save changes
            var bookToUpdateDto = await db.Books
              .Select(b => new BookTitleDto { Title = b.Title })
              .FirstAsync(b => b.Title == "Animal Farm");

            bookToUpdateDto.Title = "Cow";
            await db.SaveChangesAsync();

            // Tracked entity will be updated on Save changes
            var bookToUpdateCorrect = await db.Books
              .FirstAsync(b => b.Title == "Animal Farm");

            bookToUpdateCorrect.Title = "Cow";
            await db.SaveChangesAsync();

            // if we use ExecuteUpdateAsync
            // Best practice to update date in db without retrieving data to app memory
            var numberOfRowsUpdated = await db.Books
                .Select(b => b.Title) // untracked query
                .Where(b => b.StartsWith("P"))
                .ExecuteUpdateAsync(b => b.SetProperty(b => b, b => b + " (Updated)"));

            // then we don't need to call save on db
            // await db.SaveChangesAsync();

            // take ExecuteUpdateAsync remarks in count
            // Remarks:
            //     This operation executes immediately against the database, rather than being deferred
            //     until Microsoft.EntityFrameworkCore.DbContext.SaveChanges is called. It also
            //     does not interact with the EF change tracker in any way: entity instances which
            //     happen to be tracked when this operation is invoked aren't taken into account,
            //     and aren't updated to reflect the changes.
        })
        .WithName("Update")
        .WithOpenApi();

        group.MapGet("/bulkupdate", async (BookStoreContext db) =>
        {
            // If we need to load all date for some work to be done outside of db,
            // and for big chunk of date we need to look in bulk update solutions
            // Behind the curtains EF Core is generating SQL statement with StringBuilder for all rows updates, so it's not efficient
            var genresToUpdate = await db.Genres.ToListAsync();

            foreach (var genre in genresToUpdate)
            {
                if (genre.Id % 2 == 0)
                    genre.Name += " (Updated)";
            }

            db.UpdateRange(genresToUpdate);
            // using some bulk update package
            // await db.BulkSaveChanges();
        })
        .WithName("bulkupdate")
        .WithOpenApi();

        group.MapGet("/firstordefault", async (BookStoreContext db) =>
        {
            // = Server side evaluation =
            var book1 = await db.Books
                .FirstOrDefaultAsync(b => b.Title == "NonExistingTitle");

            var book2 = await db.Books
                .Where(b => b.Title == "NonExistingTitle")
                .FirstOrDefaultAsync();
            // Both queries are translated to only use FirstOrDefault in SQL

            // = Client side evaluation =
            // this is not EF Core learning application but for educational purposes
            var localBooks = await db.Books.ToListAsync();

            // This will find first or default in localBooks in memory
            var book3 = localBooks
                .FirstOrDefault(b => b.Title == "NonExistingTitle");

            // This will filter all books in localBooks in memory and then find first or default
            // More work for CPU and memory
            var book4 = localBooks
                .Where(b => b.Title == "NonExistingTitle")
                .FirstOrDefault();
        })
       .WithName("FirstOrDefault")
       .WithOpenApi();

        group.MapGet("/clientvsserverside", async (BookStoreContext db) =>
        {
            var blogs = await db.Books
                .Where(b => b.Price > 3)            // SERVER: Translated to SQL
                .AsAsyncEnumerable()                // TRANSITION POINT: Switch to client-side
                .Where(b => SomeLocalFunction(b))   // CLIENT: Executed in .NET
                .Where(b => b.Title.Contains("EF")) // CLIENT: Also executed in .NET (NOT SQL)
                .ToListAsync();
        })
        .WithName("Client vs Server side")
        .WithOpenApi();

        group.MapGet("/streaming", async (BookStoreContext db) =>
        {
            // = Synchronous Streaming =
            foreach (var subModel in db.Books
                                        .Where(sm => sm.Title.StartsWith("S"))
                                        .AsNoTracking())
            {
                // Process one record at a time
                // Keeps the connection open throughout
            }

            // = Asynchronous Streaming =
            await foreach (var subModel in db.Books
                                            .Where(sm => sm.Title.StartsWith("S"))
                                            .AsNoTracking()
                                            .AsAsyncEnumerable())
            {
                // Process one record at a time, non-blocking
                // AsAsyncEnumerable() doesn't execute one query per item, opens a DataReader with the full query and streams results from that single reader
                // Keeps the connection open throughout the enumeration
            }

            var bufferedFilter = db.Books
                .Where(sm => sm.Title.StartsWith("S"))
                .ToList() // Buffers all results here
                .Where(p => SomeLocalFunction(p));

            var streamedFilter = db.Books
                .Where(sm => sm.Title.StartsWith("S"))
                .AsEnumerable() // Streaming request
                .Where(p => SomeLocalFunction(p));

            // True streaming behavior depends on both EF Core's query translation and how your database handles the query.
            // Always profile and test with realistic data volumes to ensure optimal performance.
        })
        .WithName("Streaming")
        .WithOpenApi();

        group.MapGet("/withidentity", async (BookStoreContext db) =>
        {
            // Sharing object reference for same entities in connected entities
            var untrackedIdentityBooksWithAuthors = await db.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
            // Explanation with examples https://claude.ai/share/9e8861a7-f37f-4be6-b636-a5791db225bb
        })
        .WithName("With Identity Resolution")
        .WithOpenApi();

        group.MapGet("/cartesianexploasion", async (BookStoreContext db) =>
        {
            // We can reduce size of data transferred from database by splitting query to multiple queries with AsSplitQuery
            // [B1 BA1  A1]
            // [B1 BA2  A2]
            // [B1 BA3  A3]
            // [B2 BA21 A1]
            // [B2 BA22 A4]
            var someBooks = await db.Books
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .OrderByDescending(sm => sm.Title)
                .ToListAsync();

            // becomes
            // [B1, B2]
            // [BA1, BA2, BA3, BA21, BA22]
            // [A1, A2, A3, A4]
            var someBooks2 = await db.Books
                .AsSplitQuery()
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .OrderByDescending(sm => sm.Title)
                .ToListAsync();
        })
        .WithName("Cartesian explosion")
        .WithOpenApi();

        group.MapGet("/dbfunctions", async (BookStoreContext db) =>
        {
            var leastCount = await db.Books
                .Select(b => EF.Functions.Least(b.PageCount, b.Price, b.Title.Length))
                .ToListAsync();
        })
        .WithName("Using DB functions")
        .WithOpenApi();

        group.MapGet("/cancellationtoken", async (BookStoreContext db, CancellationToken ct) =>
        {
            // Cancellation token can be used to cancel long running queries
            var book1 = await db.Books
                .Where(b => b.Title.StartsWith("P"))
                .ToListAsync(ct);
        })
       .WithName("Cancellation token")
       .WithOpenApi();

        group.MapGet("/globalqueryfilters", async (BookStoreContext db, CancellationToken ct) =>
        {
            // Gets filtered data based on global query filters
            var filteredData = await db.Books.ToListAsync(ct);

            // Gets ALL data by ignoring any global query filters
            var allData = await db.Books
                .IgnoreQueryFilters()
                .ToListAsync(ct);

            // Configuration of query filters is in public void Configure(EntityTypeBuilder<Genre> builder)
        })
        .WithName("Global query Filters")
        .WithOpenApi();

        group.MapGet("/enumproblem", (BookStoreContext db, CancellationToken ct) =>
        {
            // By default EF will not send default enum value to database
            // But EF core (CLR) default value for enum is 0 where with db we can set different default value

            // Ef core provided HasSentinel to solve this problem
            // configuration is in public void Configure(EntityTypeBuilder<Book> builder)
            // .HasSentinel(BookStatus.Unspecified); is telling the EF to treat Unspecified as the "no value" state and DB can provide it's default value
            // but if you get our enum default value 0 = InStore, you know that it was not set in the application and DB provided it's default value
        })
        .WithName("Enum problem")
        .WithOpenApi();

        group.MapGet("/rowversions", (BookStoreContext db, CancellationToken ct) =>
        {
            // Configuration is in public void Configure(EntityTypeBuilder<Author> builder)

            // SQL Server automatic optimistic concurrency is handled using rowversion columns.
            // They're used for optimistic concurrency control - a way to detect if data has changed
            // between when you read it and when you try to update it.
            // Think of it like a version number that gets bumped every time someone changes a record

            // A rowversion is an 8 - byte opaque value passed between database, client, and server. 
        })
        .WithName("Row versions")
        .WithOpenApi();

    }
}
