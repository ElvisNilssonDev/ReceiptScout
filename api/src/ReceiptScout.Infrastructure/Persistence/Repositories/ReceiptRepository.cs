using Microsoft.EntityFrameworkCore;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Infrastructure.Persistence.Repositories;

public class ReceiptRepository : GenericRepository<Receipt>, IReceiptRepository
{
    public ReceiptRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Receipt>> GetByUserIdAsync(string userId)
        => await DbSet.Where(r => r.UserId == userId).ToListAsync();

    public async Task<Receipt?> GetByIdWithDetailsAsync(Guid id)
        => await DbSet
            .Include(r => r.Category)
            .Include(r => r.ExpenseReport)
            .FirstOrDefaultAsync(r => r.Id == id);
}