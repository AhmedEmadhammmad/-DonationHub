using DonationHub.Domain.Entities;

namespace DonationHub.Application.Common.Interfaces;

/// <summary>
/// Service interface responsible for cryptographic JWT token generation and refresh token hashing.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Issues a signed, short-lived JWT access token containing standard identity claims.
    /// </summary>
    string GenerateAccessToken(ApplicationUser user);

    /// <summary>
    /// Generates a cryptographically random refresh token string and computes its SHA-256 hash.
    /// </summary>
    (string RawToken, string TokenHash) GenerateRefreshToken();

    /// <summary>
    /// Computes the SHA-256 hash representation of a raw token string for secure database lookup.
    /// </summary>
    string HashToken(string rawToken);
}

