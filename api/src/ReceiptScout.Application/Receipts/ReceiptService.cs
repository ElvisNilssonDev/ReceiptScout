using ReceiptScout.Application.Common.Exceptions;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Application.Receipts.Dtos;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Exceptions;

namespace ReceiptScout.Application.Receipts;

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public ReceiptService(IReceiptRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<ReceiptResponse> CreateAsync(CreateReceiptDto dto)
    {
        var userId = RequireUserId();

        // The DOMAIN constructor enforces invariants one more time —
        // even if FluentValidation was bypassed, a bad receipt cannot exist.
        var receipt = new Receipt(
            dto.Merchant,
            dto.Date,
            dto.TotalAmount,
            dto.VatAmount,
            userId,
            dto.Description,
            dto.ImageUrl);

        if (dto.CategoryId.HasValue) receipt.AssignCategory(dto.CategoryId.Value);
        if (dto.ExpenseReportId.HasValue) receipt.AssignToReport(dto.ExpenseReportId.Value);

        await _repository.AddAsync(receipt);
        await _repository.SaveChangesAsync();

        return Map(receipt);
    }

    public async Task<ReceiptResponse> GetByIdAsync(Guid id)
    {
        var receipt = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Receipt), id);

        return Map(receipt);
    }

    public async Task<IReadOnlyList<ReceiptResponse>> GetAllForCurrentUserAsync()
    {
        var userId = RequireUserId();
        var receipts = await _repository.GetByUserIdAsync(userId);
        return receipts.Select(Map).ToList();
    }

    public async Task<ReceiptResponse> UpdateAsync(Guid id, UpdateReceiptDto dto)
    {
        var receipt = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Receipt), id);

        EnsureOwnerOrAdmin(receipt);

        // Mutation goes through the entity's own method — invariants enforced
        receipt.UpdateDetails(
            dto.Merchant,
            dto.Date,
            dto.TotalAmount,
            dto.VatAmount,
            dto.Description,
            dto.ImageUrl,
            dto.CategoryId,
            dto.ExpenseReportId);

        _repository.Update(receipt);
        await _repository.SaveChangesAsync();

        return Map(receipt);
    }

    public async Task DeleteAsync(Guid id)
    {
        var receipt = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Receipt), id);

        EnsureOwnerOrAdmin(receipt);

        _repository.Remove(receipt);
        await _repository.SaveChangesAsync();
    }

    // --- Auth helpers ---

    private string RequireUserId() =>
        _currentUser.UserId ?? throw new ForbiddenException("No authenticated user.");

    private void EnsureOwnerOrAdmin(Receipt receipt)
    {
        if (receipt.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenException();
    }

    // --- Mapping (manual; AutoMapper would be overkill at this size) ---

    private static ReceiptResponse Map(Receipt r) => new(
        r.Id, r.Merchant, r.Date, r.TotalAmount, r.VatAmount,
        r.Description, r.ImageUrl, r.UserId, r.CategoryId, r.ExpenseReportId, r.CreatedAt);
}
