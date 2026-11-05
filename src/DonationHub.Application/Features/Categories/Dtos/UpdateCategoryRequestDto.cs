using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Categories.Dtos;

/// <summary>
/// Data transfer object carrying input for modifying an existing category (CAT-01 / CAT-02).
/// </summary>
public class UpdateCategoryRequestDto
{
    /// <summary>
    /// Required updated Arabic category name (max 100 characters).
    /// </summary>
    [Required(ErrorMessage = "Arabic name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Arabic name must be between 2 and 100 characters.")]
    public string NameAr { get; init; } = string.Empty;

    /// <summary>
    /// Required updated English category name (max 100 characters).
    /// </summary>
    [Required(ErrorMessage = "English name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "English name must be between 2 and 100 characters.")]
    public string NameEn { get; init; } = string.Empty;

    /// <summary>
    /// Optional updated Arabic description (max 500 characters).
    /// </summary>
    [StringLength(500, ErrorMessage = "Arabic description cannot exceed 500 characters.")]
    public string? DescriptionAr { get; init; }

    /// <summary>
    /// Optional updated English description (max 500 characters).
    /// </summary>
    [StringLength(500, ErrorMessage = "English description cannot exceed 500 characters.")]
    public string? DescriptionEn { get; init; }
}

