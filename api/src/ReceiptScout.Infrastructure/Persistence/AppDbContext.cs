using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Identity;

namespace ReceiptScout.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ExpenseReport> ExpenseReports => Set<ExpenseReport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
