using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Auth.Dtos;

/// <summary>
/// Data transfer object carrying user login credentials.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Registered user email identifier.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User account password.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = string.Empty;
}

