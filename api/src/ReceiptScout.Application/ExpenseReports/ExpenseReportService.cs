using ReceiptScout.Application.Common.Exceptions;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Application.ExpenseReports.Dtos;
using ReceiptScout.Application.Receipts.Dtos;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Exceptions;

namespace ReceiptScout.Application.ExpenseReports;

public class ExpenseReportService : IExpenseReportService
{
    private readonly IExpenseReportRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public ExpenseReportService(
        IExpenseReportRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ExpenseReportResponse> CreateAsync(CreateExpenseReportDto dto)
    {
        var userId = RequireUserId();
        var report = new ExpenseReport(dto.Title, userId);
        await _repository.AddAsync(report);
        await _repository.SaveChangesAsync();
        return Map(report);
    }

    public async Task<ExpenseReportResponse> GetByIdAsync(Guid id)
    {
        var report = await Load(id);
        return Map(report);
    }

    public async Task<IReadOnlyList<ExpenseReportResponse>> GetAllForCurrentUserAsync()
    {
        var userId = RequireUserId();

        // Admin ser alla rapporter (godkännandekön); vanliga användare ser bara sina egna.//
        var reports = _currentUser.IsAdmin
            ? await _repository.GetAllAsync()
            : await _repository.GetByUserIdAsync(userId);

        return reports.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<ReceiptResponse>> GetReceiptsAsync(Guid id)
    {
        var report = await _repository.GetByIdWithReceiptsAsync(id)
            ?? throw new NotFoundException(nameof(ExpenseReport), id);

        EnsureOwnerOrAdmin(report);

        return report.Receipts.Select(MapReceipt).ToList();
    }

    public async Task<ExpenseReportResponse> UpdateAsync(Guid id, UpdateExpenseReportDto dto)
    {
        var report = await Load(id);
        EnsureOwnerOrAdmin(report);
        report.UpdateTitle(dto.Title);
        _repository.Update(report);
        await _repository.SaveChangesAsync();
        return Map(report);
    }

    public async Task DeleteAsync(Guid id)
    {
        var report = await Load(id);
        EnsureOwnerOrAdmin(report);
        _repository.Remove(report);
        await _repository.SaveChangesAsync();
    }

    public async Task<ExpenseReportResponse> SubmitAsync(Guid id)
    {
        var report = await Load(id);
        EnsureOwner(report);
        report.Submit();
        _repository.Update(report);
        await _repository.SaveChangesAsync();
        return Map(report);
    }

    public async Task<ExpenseReportResponse> ApproveAsync(Guid id)
    {
        var adminId = RequireUserId();
        EnsureAdmin();
        var report = await Load(id);
        report.Approve(adminId);
        _repository.Update(report);
        await _repository.SaveChangesAsync();
        return Map(report);
    }

    public async Task<ExpenseReportResponse> RejectAsync(Guid id)
    {
        var adminId = RequireUserId();
        EnsureAdmin();
        var report = await Load(id);
        report.Reject(adminId);
        _repository.Update(report);
        await _repository.SaveChangesAsync();
        return Map(report);
    }

    private async Task<ExpenseReport> Load(Guid id) =>
        await _repository.GetByIdAsync(id)
        ?? throw new NotFoundException(nameof(ExpenseReport), id);

    private string RequireUserId() =>
        _currentUser.UserId ?? throw new ForbiddenException("No authenticated user.");

    private void EnsureOwner(ExpenseReport report)
    {
        if (report.UserId != _currentUser.UserId) throw new ForbiddenException();
    }

    private void EnsureOwnerOrAdmin(ExpenseReport report)
    {
        if (report.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenException();
    }

    private void EnsureAdmin()
    {
        if (!_currentUser.IsAdmin) throw new ForbiddenException();
    }

    private static ExpenseReportResponse Map(ExpenseReport r) => new(
        r.Id, r.Title, r.Status, r.UserId, r.ApprovedByAdminId, r.CreatedAt, r.SubmittedAt);

    private static ReceiptResponse MapReceipt(Receipt r) => new(
        r.Id, r.Merchant, r.Date, r.TotalAmount, r.VatAmount,
        r.Description, r.ImageUrl, r.UserId, r.CategoryId, r.ExpenseReportId, r.CreatedAt);
}