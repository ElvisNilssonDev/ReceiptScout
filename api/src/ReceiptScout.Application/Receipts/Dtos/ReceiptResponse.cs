using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.Receipts.Dtos;

public record ReceiptResponse(
    Guid Id,
    string Merchant,
    DateTime Date,
    decimal TotalAmount,
    decimal VatAmount,
    string? Description,
    string? ImageUrl,
    string UserId,
    Guid? CategoryId,
    Guid? ExpenseReportId,
    DateTime CreatedAt);
