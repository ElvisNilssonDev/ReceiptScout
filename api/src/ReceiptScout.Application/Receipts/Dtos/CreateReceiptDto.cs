using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.Receipts.Dtos;

public record CreateReceiptDto(
    string Merchant,
    DateTime Date,
    decimal TotalAmount,
    decimal VatAmount,
    string? Description,
    string? ImageUrl,
    Guid? CategoryId,
    Guid? ExpenseReportId);