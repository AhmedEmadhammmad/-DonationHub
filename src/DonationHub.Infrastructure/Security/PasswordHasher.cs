using System.Security.Cryptography;
using DonationHub.Application.Common.Interfaces;

namespace DonationHub.Infrastructure.Security;

/// <summary>
/// Cryptographic password hasher utilizing PBKDF2 with HMAC-SHA256 and cryptographic salts.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
    private const char SegmentDelimiter = ':';

    /// <summary>
    /// Hashes plain-text password using a generated random salt and PBKDF2 key derivation.
    /// </summary>
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return string.Join(
            SegmentDelimiter,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash),
            Iterations,
            Algorithm.Name);
    }

    /// <summary>
    /// Validates password authenticity against the encoded salt and hash segments.
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        string[] segments = passwordHash.Split(SegmentDelimiter);
        if (segments.Length != 4)
            return false;

        byte[] salt = Convert.FromBase64String(segments[0]);
        byte[] originalHash = Convert.FromBase64String(segments[1]);
        if (!int.TryParse(segments[2], out int iterations))
            return false;

        string algorithmName = segments[3];
        var algorithm = new HashAlgorithmName(algorithmName);

        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithm, originalHash.Length);

        return CryptographicOperations.FixedTimeEquals(originalHash, computedHash);
    }
}

