using ReceiptScout.Application.Ai.Dtos;

namespace ReceiptScout.Application.Ai;

/// <summary>
/// The swappable AI provider seam. The Application layer depends only on this;
/// Infrastructure supplies the implementation (a stub today, Google Gemini later).
/// Switching providers is a single DI registration change.
/// </summary>
public interface IAiCategorizationService
{
    Task<CategorySuggestion> SuggestCategoryAsync(
        ReceiptCategorizationRequest request,
        CancellationToken ct = default);
}