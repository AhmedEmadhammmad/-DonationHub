namespace DonationHub.Application.Features.Categories.Dtos;

/// <summary>
/// Data transfer object representing a bilingual category payload.
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Unique category identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Arabic display name.
    /// </summary>
    public string NameAr { get; init; } = string.Empty;

    /// <summary>
    /// English display name.
    /// </summary>
    public string NameEn { get; init; } = string.Empty;

    /// <summary>
    /// Optional Arabic description.
    /// </summary>
    public string? DescriptionAr { get; init; }

    /// <summary>
    /// Optional English description.
    /// </summary>
    public string? DescriptionEn { get; init; }

    /// <summary>
    /// Record creation timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Record last update timestamp in UTC.
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}

