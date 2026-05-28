using Microsoft.EntityFrameworkCore;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsByNameAsync(string name)
        => await DbSet.AnyAsync(c => c.Name == name);

    public async Task<Category?> GetBySlugAsync(string slug)
        => await DbSet.FirstOrDefaultAsync(c => c.Slug == slug);
}