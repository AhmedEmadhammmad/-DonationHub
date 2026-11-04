namespace DonationHub.Application.Features.Auth.Dtos;

/// <summary>
/// Data transfer object representing safe user identity profile without exposing credentials.
/// </summary>
public class AuthUserDto
{
    /// <summary>
    /// Unique user identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User display full name.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// User assigned authorization role.
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Preferred interface language code.
    /// </summary>
    public string PreferredLanguage { get; init; } = string.Empty;
}

