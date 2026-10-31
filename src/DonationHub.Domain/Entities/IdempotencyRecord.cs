using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class IdempotencyRecord
{
    public string Key { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public string RequestHash { get; private set; } = string.Empty;
    public int ResponseStatusCode { get; private set; }
    public string ResponseBody { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private IdempotencyRecord()
    {
    }

    public static IdempotencyRecord Create(
        string key,
        Guid userId,
        string requestHash,
        int responseStatusCode,
        string responseBody,
        DateTime nowUtc,
        DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Idempotency key cannot be empty.");

        if (userId == Guid.Empty)
            throw new DomainException("UserId cannot be empty.");

        if (string.IsNullOrWhiteSpace(requestHash))
            throw new DomainException("RequestHash cannot be empty.");

        if (expiresAtUtc <= nowUtc)
            throw new DomainException("Expiration date must be in the future.");

        return new IdempotencyRecord
        {
            Key = key.Trim(),
            UserId = userId,
            RequestHash = requestHash.Trim(),
            ResponseStatusCode = responseStatusCode,
            ResponseBody = responseBody ?? string.Empty,
            CreatedAt = nowUtc,
            ExpiresAt = expiresAtUtc
        };
    }

    public bool IsExpired(DateTime nowUtc) => nowUtc >= ExpiresAt;
}

