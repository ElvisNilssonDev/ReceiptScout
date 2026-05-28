using System;
using System.Collections.Generic;
using System.Text;

using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Application.Common.Interfaces;

public interface IReceiptRepository : IGenericRepository<Receipt>
{
    Task<IReadOnlyList<Receipt>> GetByUserIdAsync(string userId);
    Task<Receipt?> GetByIdWithDetailsAsync(Guid id);
}