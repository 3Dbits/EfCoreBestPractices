using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EfCoreBP.ApiService.Models;

namespace EfCoreBP.ApiService.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(a => a.Biography)
            .HasMaxLength(2000);
            
        builder.Property(a => a.DateOfBirth)
            .IsRequired();
            
        // Index on email for faster lookups
        builder.HasIndex(a => a.Email)
            .IsUnique()
            .HasDatabaseName("IX_Authors_Email");
            
        // Index on last name for faster searches
        builder.HasIndex(a => a.LastName)
            .HasDatabaseName("IX_Authors_LastName");

        builder.Property(e => e.RowVersion)
            .IsRowVersion();
    }
}