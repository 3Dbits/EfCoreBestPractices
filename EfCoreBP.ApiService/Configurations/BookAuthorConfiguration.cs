using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EfCoreBP.ApiService.Models;

namespace EfCoreBP.ApiService.Configurations;

public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
{
    public void Configure(EntityTypeBuilder<BookAuthor> builder)
    {
        builder.ToTable("BookAuthors");
        
        // Composite primary key
        builder.HasKey(ba => new { ba.BookId, ba.AuthorId });
        
        builder.Property(ba => ba.IsPrimaryAuthor)
            .IsRequired();
            
        builder.Property(ba => ba.ContributionDate)
            .IsRequired();
            
        // Configure relationships
        builder.HasOne(ba => ba.Book)
            .WithMany(b => b.BookAuthors)
            .HasForeignKey(ba => ba.BookId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(ba => ba.Author)
            .WithMany(a => a.BookAuthors)
            .HasForeignKey(ba => ba.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Index for faster queries
        builder.HasIndex(ba => ba.BookId)
            .HasDatabaseName("IX_BookAuthors_BookId");
            
        builder.HasIndex(ba => ba.AuthorId)
            .HasDatabaseName("IX_BookAuthors_AuthorId");
    }
}