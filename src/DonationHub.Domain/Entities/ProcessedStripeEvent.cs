using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class ProcessedStripeEvent
{
    public string EventId { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public DateTime ProcessedAt { get; private set; }

    private ProcessedStripeEvent()
    {
    }

    public static ProcessedStripeEvent Create(string eventId, string type, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            throw new DomainException("Stripe EventId cannot be empty.");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Stripe event Type cannot be empty.");

        return new ProcessedStripeEvent
        {
            EventId = eventId.Trim(),
            Type = type.Trim(),
            ProcessedAt = nowUtc
        };
    }
}

