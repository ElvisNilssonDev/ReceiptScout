using ReceiptScout.Domain.Identity;

namespace ReceiptScout.Domain.Entities;

public class Receipt
{
    // Parameterless ctor for EF Core materialization only.
    // Private so application code is forced through the validating ctor below.
    private Receipt() { }

    public Receipt(
        string merchant,
        DateTime date,
        decimal totalAmount,
        decimal vatAmount,
        string userId,
        string? description = null,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(merchant))
            throw new ArgumentException("Merchant is required.", nameof(merchant));

        if (totalAmount <= 0)
            throw new ArgumentException("Total amount must be greater than zero.", nameof(totalAmount));

        if (vatAmount < 0)
            throw new ArgumentException("VAT amount cannot be negative.", nameof(vatAmount));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        Id = Guid.NewGuid();
        Merchant = merchant;
        Date = date;
        TotalAmount = totalAmount;
        VatAmount = vatAmount;
        UserId = userId;
        Description = description;
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Merchant { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal VatAmount { get; private set; }
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Foreign keys
    public string UserId { get; private set; } = null!;
    public Guid? CategoryId { get; private set; }
    public Guid? ExpenseReportId { get; private set; }


    // Navigation properties
    public ApplicationUser User { get; private set; } = null!;
    public Category? Category { get; private set; }
    public ExpenseReport? ExpenseReport { get; private set; }


}