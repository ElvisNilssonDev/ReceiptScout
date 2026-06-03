using ReceiptScout.Application.Ai;
using ReceiptScout.Application.Ai.Dtos;

namespace ReceiptScout.Infrastructure.Ai;

/// <summary>
/// Placeholder implementation of <see cref="IAiCategorizationService"/>. Does a
/// naive keyword match against the candidate categories so the endpoint returns
/// something believable today. Replaced by the real Gemini client on Days 13–14 —
/// only the DI registration changes, nothing in Application or Domain.
/// </summary>
public class StubAiCategorizationService : IAiCategorizationService
{
    public Task<CategorySuggestion> SuggestCategoryAsync(
        ReceiptCategorizationRequest request,
        CancellationToken ct = default)
    {
        var haystack = $"{request.Merchant} {request.Description}".ToLowerInvariant();

        var match = request.Candidates.FirstOrDefault(c =>
            c.Name.ToLowerInvariant()
             .Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Any(word => word.Length > 3 && haystack.Contains(word)));

        if (match is not null)
        {
            return Task.FromResult(new CategorySuggestion(
                match.CategoryId,
                match.BasAccount,
                0.35,
                $"Placeholder keyword match for '{request.Merchant}'. Replace with Gemini (Day 13–14)."));
        }

        var fallback = request.Candidates.FirstOrDefault();
        return Task.FromResult(new CategorySuggestion(
            fallback?.CategoryId,
            fallback?.BasAccount ?? "6991",
            0.0,
            "Placeholder stub — no confident match. Replace with Gemini (Day 13–14)."));
    }
}