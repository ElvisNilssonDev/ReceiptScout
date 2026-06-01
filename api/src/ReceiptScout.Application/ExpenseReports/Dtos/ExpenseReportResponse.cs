using ReceiptScout.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.ExpenseReports.Dtos
{
    public record ExpenseReportResponse(
    Guid Id,
    string Title,
    ExpenseReportStatus Status,
    string UserId,
    string? ApprovedByAdminId,
    DateTime CreatedAt,
    DateTime? SubmittedAt);

}
