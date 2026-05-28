using System;
using System.Collections.Generic;
using System.Text;

using ReceiptScout.Domain.Entities;

namespace ReceiptScout.Application.Common.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> ExistsByNameAsync(string name);
    Task<Category?> GetBySlugAsync(string slug);
}
