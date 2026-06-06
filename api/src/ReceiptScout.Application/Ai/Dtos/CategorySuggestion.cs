namespace ReceiptScout.Application.Ai.Dtos;

/// <summary>
/// The AI's suggestion. <see cref="CategoryId"/> is null when nothing matched
/// confidently, but <see cref="BasAccount"/> is always populated so the caller
/// still has something to fall back on.
/// </summary>
public record CategorySuggestion(
    Guid? CategoryId,
    string BasAccount,
    double Confidence,
    string Rationale);