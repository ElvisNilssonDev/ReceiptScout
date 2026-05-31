using System;
using System.Collections.Generic;
using System.Text;
using ReceiptScout.Application.Receipts.Dtos;

namespace ReceiptScout.Application.Receipts;

public interface IReceiptService
{
    Task<ReceiptResponse> CreateAsync(CreateReceiptDto dto);
    Task<ReceiptResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ReceiptResponse>> GetAllForCurrentUserAsync();
    Task<ReceiptResponse> UpdateAsync(Guid id, UpdateReceiptDto dto);
    Task DeleteAsync(Guid id);
}
