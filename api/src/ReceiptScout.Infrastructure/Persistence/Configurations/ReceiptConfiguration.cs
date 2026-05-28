using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Infrastructure.Persistence.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Merchant).IsRequired().HasMaxLength(200);
        builder.Property(r => r.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(r => r.VatAmount).HasColumnType("decimal(18,2)");
        builder.Property(r => r.Description).HasMaxLength(1000);
        builder.Property(r => r.ImageUrl).HasMaxLength(500);

        // Receipt → User (required). Restrict: radera inte finansiella poster
        // automatiskt om en användare tas bort.
        builder.HasOne(r => r.User)
            .WithMany(u => u.Receipts) 
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Receipt → Category (optional). SetNull: tar man bort en kategori
        // blir kvittona bara okategoriserade, inte raderade.
        builder.HasOne(r => r.Category)
            .WithMany(c => c.Receipts)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Receipt → ExpenseReport (optional). SetNull: tar man bort en rapport
        // lossnar kvittona men överlever.
        builder.HasOne(r => r.ExpenseReport)
            .WithMany(e => e.Receipts)
            .HasForeignKey(r => r.ExpenseReportId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}