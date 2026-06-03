using ReceiptScout.Application.Ai.Dtos;

namespace ReceiptScout.Application.Ai;

/// <summary>
/// Application use case: load a receipt, ground the AI with the available
/// categories, and return its suggestion. This is the consumer of the AI seam.
/// </summary>
public interface IReceiptCategorizationService
{
    Task<CategorySuggestion> SuggestForReceiptAsync(Guid receiptId, CancellationToken ct = default);
}