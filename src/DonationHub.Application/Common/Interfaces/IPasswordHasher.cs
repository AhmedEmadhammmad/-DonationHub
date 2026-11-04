namespace DonationHub.Application.Common.Interfaces;

/// <summary>
/// Abstraction for cryptographic password hashing and verification.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password using a salted cryptographic algorithm.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies whether a plain-text password matches an existing salted hash.
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}

