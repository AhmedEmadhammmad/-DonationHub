using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Auth.Dtos;

/// <summary>
/// Data transfer object carrying donor registration inputs.
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// User email address used as primary login identifier.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User full name.
    /// </summary>
    [Required(ErrorMessage = "FullName is required.")]
    [MaxLength(150)]
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Account password adhering to IDN-02 complexity requirements (8+ chars, upper, lower, digit).
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Preferred interface language code ('en' or 'ar', defaults to 'en').
    /// </summary>
    public string PreferredLanguage { get; init; } = "en";
}

