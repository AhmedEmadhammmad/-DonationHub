namespace DonationHub.Application.Features.Auth.Dtos;

/// <summary>
/// Data transfer object returned upon successful authentication containing JWT and refresh token.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Signed JWT access token used in Authorization Bearer header.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Cryptographic refresh token string used to request updated access tokens.
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Lifespan duration in seconds for the access token.
    /// </summary>
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Standard token scheme identifier (defaults to "Bearer").
    /// </summary>
    public string TokenType { get; init; } = "Bearer";

    /// <summary>
    /// Safe user profile claims representation.
    /// </summary>
    public AuthUserDto User { get; init; } = default!;
}
