using System;
using System.Collections.Generic;
using System.Text;
using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Application.Common.Interfaces;

public interface IExpenseReportRepository : IGenericRepository<ExpenseReport>
{
    Task<IReadOnlyList<ExpenseReport>> GetByUserIdAsync(string userId);
    Task<ExpenseReport?> GetByIdWithReceiptsAsync(Guid id);
}

