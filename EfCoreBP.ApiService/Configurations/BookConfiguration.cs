using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EfCoreBP.ApiService.Models;

namespace EfCoreBP.ApiService.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(b => b.Description)
            .HasMaxLength(2000);
            
        builder.Property(b => b.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(b => b.PageCount)
            .IsRequired();
            
        builder.Property(b => b.PublishedDate)
            .IsRequired();
            
        // Foreign key configuration
        builder.HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(b => b.ISBN)
            .IsUnique()
            .HasDatabaseName("IX_Books_ISBN");
            
        builder.HasIndex(b => b.Title)
            .HasDatabaseName("IX_Books_Title");
            
        builder.HasIndex(b => b.GenreId)
            .HasDatabaseName("IX_Books_GenreId");
    }
}