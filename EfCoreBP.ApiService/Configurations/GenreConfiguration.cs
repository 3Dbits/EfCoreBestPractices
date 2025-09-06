using EfCoreBP.ApiService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreBP.ApiService.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");
        
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.Id)
            .ValueGeneratedOnAdd();

        builder.Property(g => g.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(g => g.Description)
            .HasMaxLength(500);
            
        // Index on name for faster lookups
        builder.HasIndex(g => g.Name)
            .IsUnique()
            .HasDatabaseName("IX_Genres_Name");

        builder.HasQueryFilter(p => p.IsActive);
    }
}