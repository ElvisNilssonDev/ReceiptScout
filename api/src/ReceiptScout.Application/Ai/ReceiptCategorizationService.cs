using ReceiptScout.Application.Ai.Dtos;
using ReceiptScout.Application.Common.Exceptions;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Exceptions;

namespace ReceiptScout.Application.Ai;

public class ReceiptCategorizationService : IReceiptCategorizationService
{
    private readonly IReceiptRepository _receipts;
    private readonly ICategoryRepository _categories;
    private readonly IAiCategorizationService _ai;
    private readonly ICurrentUserService _currentUser;

    public ReceiptCategorizationService(
        IReceiptRepository receipts,
        ICategoryRepository categories,
        IAiCategorizationService ai,
        ICurrentUserService currentUser)
    {
        _receipts = receipts;
        _categories = categories;
        _ai = ai;
        _currentUser = currentUser;
    }

    public async Task<CategorySuggestion> SuggestForReceiptAsync(Guid receiptId, CancellationToken ct = default)
    {
        var receipt = await _receipts.GetByIdAsync(receiptId)
            ?? throw new NotFoundException(nameof(Receipt), receiptId);

        // Same owner-or-admin rule as the rest of the receipt surface.
        if (receipt.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenException();

        var candidates = (await _categories.GetAllAsync())
            .Select(c => new CategoryOption(c.Id, c.Name, c.BasAccount, c.VatRate))
            .ToList();

        var request = new ReceiptCategorizationRequest(
            receipt.Merchant,
            receipt.TotalAmount,
            receipt.VatAmount,
            receipt.Description,
            candidates);

        return await _ai.SuggestCategoryAsync(request, ct);
    }
}