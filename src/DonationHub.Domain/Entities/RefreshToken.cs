using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    private RefreshToken()
    {
    }

    public static RefreshToken Create(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTime nowUtc,
        DateTime expiresAtUtc)
    {
        if (id == Guid.Empty)
            throw new DomainException("RefreshToken Id cannot be empty.");

        if (userId == Guid.Empty)
            throw new DomainException("UserId cannot be empty.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("TokenHash cannot be empty.");

        if (expiresAtUtc <= nowUtc)
            throw new DomainException("RefreshToken expiration date must be in the future.");

        return new RefreshToken
        {
            Id = id,
            UserId = userId,
            TokenHash = tokenHash.Trim(),
            CreatedAt = nowUtc,
            ExpiresAt = expiresAtUtc,
            RevokedAt = null,
            ReplacedByTokenId = null
        };
    }

    public void Revoke(DateTime nowUtc, Guid? replacedByTokenId = null)
    {
        if (RevokedAt != null)
            throw new DomainException("RefreshToken is already revoked.");

        RevokedAt = nowUtc;
        ReplacedByTokenId = replacedByTokenId;
    }

    public bool IsExpired(DateTime nowUtc) => nowUtc >= ExpiresAt;

    public bool IsActive(DateTime nowUtc) => RevokedAt == null && !IsExpired(nowUtc);
}

