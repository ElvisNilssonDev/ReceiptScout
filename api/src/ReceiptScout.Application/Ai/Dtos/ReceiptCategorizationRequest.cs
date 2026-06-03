namespace ReceiptScout.Application.Ai.Dtos;

/// <summary>
/// Everything the AI provider needs to reason about a single receipt. The
/// <see cref="Candidates"/> list grounds the model to categories that actually
/// exist, so it cannot invent an arbitrary BAS account.
/// </summary>
public record ReceiptCategorizationRequest(
    string Merchant,
    decimal TotalAmount,
    decimal VatAmount,
    string? Description,
    IReadOnlyList<CategoryOption> Candidates);