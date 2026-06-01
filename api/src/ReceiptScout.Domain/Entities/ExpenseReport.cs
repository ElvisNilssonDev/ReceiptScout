using ReceiptScout.Domain.Enums;
using ReceiptScout.Domain.Identity;

namespace ReceiptScout.Domain.Entities;

public class ExpenseReport
{
    private ExpenseReport() { }

    public ExpenseReport(string title, string userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        Id = Guid.NewGuid();
        Title = title;
        UserId = userId;
        Status = ExpenseReportStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public ExpenseReportStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SubmittedAt { get; private set; }

    // Foreign keys — two relationships to the SAME user table
    public string UserId { get; private set; } = null!;
    public string? ApprovedByAdminId { get; private set; }

    // Navigation properties
    public ApplicationUser User { get; private set; } = null!;
    public ApplicationUser? ApprovedByAdmin { get; private set; }
    public ICollection<Receipt> Receipts { get; private set; } = new List<Receipt>();

    // --- State transitions (the state machine lives here) ---

    public void Submit()
    {
        if (Status != ExpenseReportStatus.Draft)
            throw new InvalidOperationException("Only draft reports can be submitted.");

        Status = ExpenseReportStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public void Approve(string adminId)
    {
        if (Status != ExpenseReportStatus.Submitted)
            throw new InvalidOperationException("Only submitted reports can be approved.");

        Status = ExpenseReportStatus.Approved;
        ApprovedByAdminId = adminId;
    }

    public void Reject(string adminId)
    {
        if (Status != ExpenseReportStatus.Submitted)
            throw new InvalidOperationException("Only submitted reports can be rejected.");

        Status = ExpenseReportStatus.Rejected;
        ApprovedByAdminId = adminId;
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (Status != ExpenseReportStatus.Draft)
            throw new InvalidOperationException("Only draft reports can be edited.");

        Title = title;
    }
}