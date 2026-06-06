using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Infrastructure.Persistence.Configurations;

public class ExpenseReportConfiguration : IEntityTypeConfiguration<ExpenseReport>
{
    public void Configure(EntityTypeBuilder<ExpenseReport> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Status).IsRequired(); // lagras som int by default

        // ExpenseReport → User (owner, required). Restrict.
        builder.HasOne(e => e.User)
            .WithMany(u => u.ExpenseReports)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // DETTA är cascade-fällan jag flaggade dag 1.
        // ExpenseReport har TVÅ FK:er till samma user-tabell (UserId + ApprovedByAdminId).
        // Om båda fick cascade → "multiple cascade paths"-fel i SQL Server.
        // ApprovedByAdmin är nullable → SetNull: tas en admin bort blir bara
        // godkännaren null, rapporten överlever. WithMany() = ingen invers-collection.
        builder.HasOne(e => e.ApprovedByAdmin)
            .WithMany()
            .HasForeignKey(e => e.ApprovedByAdminId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}