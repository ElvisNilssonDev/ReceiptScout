namespace ReceiptScout.Application.Ai.Dtos;

/// <summary>A candidate category the AI is allowed to choose from (grounding).</summary>
public record CategoryOption(Guid CategoryId, string Name, string BasAccount, decimal VatRate);