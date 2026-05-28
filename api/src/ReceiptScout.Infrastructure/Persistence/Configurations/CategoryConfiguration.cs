using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(120);
        builder.Property(c => c.BasAccount).IsRequired().HasMaxLength(10);
        builder.Property(c => c.VatRate).HasColumnType("decimal(5,4)");

        
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.Slug).IsUnique();
    }
}