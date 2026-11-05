using DonationHub.Domain.Common;
using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class Category : ISoftDeletable
{
    public const int MaxNameLength = 100;

    public Guid Id { get; private set; }
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Soft deletion flag modified by SoftDeleteInterceptor.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Timestamp recording when soft deletion occurred.
    /// </summary>
    public DateTime? DeletedAtUtc { get; set; }

    private Category()
    {
    }

    public static Category Create(
        Guid id,
        string nameAr,
        string nameEn,
        DateTime nowUtc,
        string? descriptionAr = null,
        string? descriptionEn = null)
    {
        if (id == Guid.Empty)
            throw new DomainException("Category Id cannot be empty.");

        ValidateNames(nameAr, nameEn);

        return new Category
        {
            Id = id,
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            DescriptionAr = descriptionAr?.Trim(),
            DescriptionEn = descriptionEn?.Trim(),
            CreatedAt = nowUtc,
            UpdatedAt = null,
            IsDeleted = false,
            DeletedAtUtc = null
        };
    }

    public void Update(
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        DateTime nowUtc)
    {
        ValidateNames(nameAr, nameEn);

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        DescriptionAr = descriptionAr?.Trim();
        DescriptionEn = descriptionEn?.Trim();
        UpdatedAt = nowUtc;
    }

    public void EnsureCanBeDeleted(bool hasCampaigns)
    {
        if (hasCampaigns)
            throw new DomainException("Cannot delete a category that has associated campaigns.");
    }

    private static void ValidateNames(string nameAr, string nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic name cannot be empty.");

        if (nameAr.Trim().Length > MaxNameLength)
            throw new DomainException($"Arabic name cannot exceed {MaxNameLength} characters.");

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English name cannot be empty.");

        if (nameEn.Trim().Length > MaxNameLength)
            throw new DomainException($"English name cannot exceed {MaxNameLength} characters.");
    }
}

