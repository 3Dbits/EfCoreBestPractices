using EfCoreBP.ApiService.Data;
using EfCoreBP.ApiService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBP.ApiService.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Books").WithTags(nameof(Book));

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
                    }).ToList()
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

        /*
          // == Untracked ==
            // When we read data without need to update it, we should use AsNoTracking or anonymous types with .Select
            var untrackedBooks = await context.Books
                .AsNoTracking()
                .ToListAsync();

            var untrackedBookNames = await context.Books
                .Select(b => new { b.Title })
                .ToListAsync();
            // .Select to an anonymous type is also untracked, we don't need AsNoTracking here

            // Best practice is to use .Select with only data(columns) that we need from database = lower size of data transferred and queries are faster
            // For that we use DTOs (Data Transfer Objects)
            var bookNameDtos = await context.Books
                .Select(b => new BookNameDto { Title = b.Title })
                .ToListAsync();

            // == Tracked ==
            // Tracked is all queries without AsNoTracking or .Select to anonymous types*
            // *Tracked object are only entities/properties that are part of DbSet in DbContext
            var trackedBooks = await context.Books
                .ToListAsync();

            var trackedBooksTitle = await context.Books
                .Select(b => b.Title)
                .ToListAsync();

            var untrackedTitles = await context.Books
                .Select(b => new BookNameDto { Title = b.Title })
                .ToListAsync();

            // we can check what is tracked in the context for debugging purposes
            // need to fix, find where did i see this
            var trackedEntities = context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged)
                .ToList();

            // why we need tracked entities?
            // if you have entities with xy columns and you want to update only one column
            // EF Core will generate UPDATE statement with all columns if we use some kind of untracked query
            var bookToUpdate = await context.Books
                .FirstAsync(b => b.Title == "The Hobbit");

            bookToUpdate.Title = "Nineteen Eighty-Four";
            await context.SaveChangesAsync();

            var bookToUpdateDto = await context.Books
               .Select(b => new BookNameDto { Title = b.Title })
               .FirstAsync(b => b.Title == "Animal Farm");

            bookToUpdateDto.Title = "Cow";
            await context.SaveChangesAsync();

            // if we use ExecuteUpdateAsync
            await context.Books
                .Select(b => b.Title) // untracked query
                .Where(b => b.StartsWith('P'))
                .ExecuteUpdateAsync(b => b.SetProperty(b => b, b => b + " (Updated)"));

            await context.SaveChangesAsync();
            // take ExecuteUpdateAsync remarks in count
            // Remarks:
            //     This operation executes immediately against the database, rather than being deferred
            //     until Microsoft.EntityFrameworkCore.DbContext.SaveChanges is called. It also
            //     does not interact with the EF change tracker in any way: entity instances which
            //     happen to be tracked when this operation is invoked aren't taken into account,
            //     and aren't updated to reflect the changes.

            // in behind curtains EF Core is generating SQL statement with StringBuilder so for many rows updates it is not efficient
            // best practice is bulk save
            var genresToUpdate = await context.Genres.ToListAsync();

            foreach (var genre in genresToUpdate)
            {
                if (genre.Id % 2 == 0)
                    genre.Name += " (Updated)";
            }

            //context.UpdateRange(genresToUpdate);
            //await context.BulkSaveChanges();


            // == FirstOrDefault ==
            // = Server side evaluation =
            var book1 = await context.Books
                .FirstOrDefaultAsync(b => b.Title == "NonExistingTitle");

            var book2 = await context.Books
                .Where(b => b.Title == "NonExistingTitle")
                .FirstOrDefaultAsync();
            // Both queries are translated to only use FirstOrDefault in SQL

            // = Client side evaluation =
            // this is not EF Core learning application but for educational purposes
            var localBooks = await context.Books.ToListAsync();

            // This will find first or default in localBooks in memory
            var book3 = localBooks
                .FirstOrDefault(b => b.Title == "NonExistingTitle");

            // This will filter all books in localBooks in memory and then find first or default
            // More work for CPU and memory
            var book4 = localBooks
                .Where(b => b.Title == "NonExistingTitle")
                .FirstOrDefault();

            // == Client vs Server side ==
            var blogs = await context.Books
                .Where(b => b.Price > 3)            // SERVER: Translated to SQL
                .AsAsyncEnumerable()                // TRANSITION POINT: Switch to client-side
                .Where(b => SomeLocalFunction(b))   // CLIENT: Executed in .NET
                .Where(b => b.Title.Contains("EF")) // CLIENT: Also executed in .NET (NOT SQL)
                .ToListAsync();

            // == Streaming ==
            // = Synchronous Streaming =
            foreach (var subModel in context.Books
                                        .Where(sm => sm.Title.StartsWith('S'))
                                        .AsNoTracking())
            {
                // Process one record at a time
                // DB call for each iteration
            }
            // = Asynchronous Streaming =
            await foreach (var subModel in context.Books
                                            .Where(sm => sm.Title.StartsWith('S'))
                                            .AsNoTracking()
                                            .AsAsyncEnumerable())
            {
                // Process one record at a time, non-blocking
                // AsAsyncEnumerable() doesn't execute one query per item, Opens a DataReader with the full query and streams results from that single reader
                // Keeps the connection open throughout the enumeration
            }

            var bufferedFilter = context.Books
                .Where(sm => sm.Title.StartsWith('S'))
                .ToList() // Buffers all results here
                .Where(p => SomeLocalFunction(p));

            var streamedFilter = context.Books
                .Where(sm => sm.Title.StartsWith('S'))
                .AsEnumerable() // Streaming request
                .Where(p => SomeLocalFunction(p));

            // True streaming behavior depends on both EF Core's query translation and how your database handles the query.
            // Always profile and test with realistic data volumes to ensure optimal performance.

            // == Performance tips ==

            // = Cartesian explosion =
            // We can reduce size of data transferred from database by splitting query to multiple queries with AsSplitQuery
            // [B1 BA1  A1]
            // [B1 BA2  A2]
            // [B1 BA3  A3]
            // [B2 BA21 A1]
            // [B2 BA22 A4]
            var someBooks = context.Books
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .OrderByDescending(sm => sm.Title)
                .ToList();

            // becomes
            // [B1, B2]
            // [BA1, BA2, BA3, BA21, BA22]
            // [A1, A2, A3, A4]
            var someBooks2 = context.Books
                .AsSplitQuery()
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .OrderByDescending(sm => sm.Title)
                .ToList();

            // = Memory =

            // Sharing reference for same entities in 
            var untrackedIdentityBooksWithAuthors = await context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
            // Explanation with examples https://claude.ai/share/9e8861a7-f37f-4be6-b636-a5791db225bb




            static bool SomeLocalFunction(Book b) => b.Title.Length > 0;
        
        
        */
    }
}
