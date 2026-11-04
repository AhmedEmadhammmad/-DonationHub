using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Features.Auth.Dtos;

/// <summary>
/// Data transfer object carrying the current refresh token for rotation.
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// Cryptographic refresh token string to rotate.
    /// </summary>
    [Required(ErrorMessage = "RefreshToken is required.")]
    public string RefreshToken { get; init; } = string.Empty;
}

