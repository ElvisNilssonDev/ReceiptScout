using System.Text.RegularExpressions;

namespace ReceiptScout.Domain.Entities;

public class Category
{
    private Category() { }

    public Category(string name, string basAccount, decimal vatRate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(basAccount))
            throw new ArgumentException("BAS account is required.", nameof(basAccount));

        Id = Guid.NewGuid();
        Name = name;
        Slug = GenerateSlug(name);
        BasAccount = basAccount;
        VatRate = vatRate;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string BasAccount { get; private set; } = null!;
    public decimal VatRate { get; private set; }

    public ICollection<Receipt> Receipts { get; private set; } = new List<Receipt>();

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant().Trim();

        // Normalize Swedish characters before stripping because well this is indeed a swedish app :D
        slug = slug
            .Replace('å', 'a').Replace('ä', 'a').Replace('ö', 'o')
            .Replace('é', 'e').Replace('ü', 'u');

        // Collapse runs of non-alphanumeric chars into a single hyphen! 
        slug = Regex.Replace(slug, "[^a-z0-9]+", "-");

        return slug.Trim('-');
    }

    public void UpdateDetails(string name, string basAccount, decimal vatRate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(basAccount))
            throw new ArgumentException("BAS account is required.", nameof(basAccount));

        Name = name;
        Slug = GenerateSlug(name);
        BasAccount = basAccount;
        VatRate = vatRate;
    }
}