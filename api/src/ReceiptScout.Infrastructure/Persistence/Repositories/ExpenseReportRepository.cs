using Microsoft.EntityFrameworkCore;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Infrastructure.Persistence.Repositories;

public class ExpenseReportRepository : GenericRepository<ExpenseReport>, IExpenseReportRepository
{
    public ExpenseReportRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ExpenseReport>> GetByUserIdAsync(string userId)
        => await DbSet.Where(e => e.UserId == userId).ToListAsync();

    public async Task<ExpenseReport?> GetByIdWithReceiptsAsync(Guid id)
        => await DbSet
            .Include(e => e.Receipts)
            .FirstOrDefaultAsync(e => e.Id == id);
}
