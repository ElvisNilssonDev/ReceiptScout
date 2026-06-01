using ReceiptScout.Application.ExpenseReports.Dtos;

namespace ReceiptScout.Application.ExpenseReports;

public interface IExpenseReportService
{
    Task<ExpenseReportResponse> CreateAsync(CreateExpenseReportDto dto);
    Task<ExpenseReportResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ExpenseReportResponse>> GetAllForCurrentUserAsync();
    Task<ExpenseReportResponse> UpdateAsync(Guid id, UpdateExpenseReportDto dto);
    Task DeleteAsync(Guid id);
    Task<ExpenseReportResponse> SubmitAsync(Guid id);
    Task<ExpenseReportResponse> ApproveAsync(Guid id);
    Task<ExpenseReportResponse> RejectAsync(Guid id);
}