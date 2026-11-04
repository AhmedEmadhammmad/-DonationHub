using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Common.Options;

/// <summary>
/// Strongly-typed configuration options for JSON Web Token (JWT) issuance and validation.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Configuration section key within configuration files.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Cryptographic signing key used for token signing and validation (minimum 32 characters).
    /// </summary>
    [Required(ErrorMessage = "JWT SecretKey is required.")]
    [MinLength(32, ErrorMessage = "JWT SecretKey must be at least 32 characters long.")]
    public string SecretKey { get; init; } = string.Empty;

    /// <summary>
    /// Intended issuer claim value for issued JWT tokens.
    /// </summary>
    [Required(ErrorMessage = "JWT Issuer is required.")]
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Intended audience claim value for issued JWT tokens.
    /// </summary>
    [Required(ErrorMessage = "JWT Audience is required.")]
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Access token lifespan duration in minutes.
    /// </summary>
    [Range(1, 1440, ErrorMessage = "ExpiryMinutes must be between 1 and 1440 minutes.")]
    public int ExpiryMinutes { get; init; } = 15;

    /// <summary>
    /// Refresh token lifespan duration in days.
    /// </summary>
    [Range(1, 90, ErrorMessage = "RefreshTokenExpiryDays must be between 1 and 90 days.")]
    public int RefreshTokenExpiryDays { get; init; } = 7;
}

