using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Auth.Dtos;

/// <summary>
/// Data transfer object for administrator-driven creation of privileged staff accounts (IDN-08).
/// </summary>
public class CreateStaffRequestDto
{
    /// <summary>
    /// Staff member corporate email address.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Staff full name.
    /// </summary>
    [Required(ErrorMessage = "FullName is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters.")]
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Initial temporary or permanent password meeting strength policy (IDN-02).
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.")]
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Assigned administrative role (restricted to Admin or CampaignManager).
    /// </summary>
    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression(@"^(Admin|CampaignManager)$", ErrorMessage = "Role must be either 'Admin' or 'CampaignManager'.")]
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Staff preferred UI language code (defaults to Arabic 'ar').
    /// </summary>
    [RegularExpression(@"^(ar|en)$", ErrorMessage = "PreferredLanguage must be 'ar' or 'en'.")]
    public string PreferredLanguage { get; init; } = "ar";
}

