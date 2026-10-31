using DonationHub.Domain.Enums;
using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class PaymentAttempt
{
    public Guid Id { get; private set; }
    public Guid DonationId { get; private set; }
    public string StripeCheckoutSessionId { get; private set; } = string.Empty;
    public string? StripePaymentIntentId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentAttemptStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? FailureCode { get; private set; }

    private PaymentAttempt()
    {
    }

    public static PaymentAttempt Create(
        Guid id,
        Guid donationId,
        string stripeCheckoutSessionId,
        decimal amount,
        DateTime nowUtc,
        string? stripePaymentIntentId = null)
    {
        if (id == Guid.Empty)
            throw new DomainException("PaymentAttempt Id cannot be empty.");

        if (donationId == Guid.Empty)
            throw new DomainException("DonationId cannot be empty.");

        if (string.IsNullOrWhiteSpace(stripeCheckoutSessionId))
            throw new DomainException("StripeCheckoutSessionId cannot be empty.");

        if (amount <= 0)
            throw new DomainException("Payment attempt amount must be strictly greater than zero.");

        return new PaymentAttempt
        {
            Id = id,
            DonationId = donationId,
            StripeCheckoutSessionId = stripeCheckoutSessionId.Trim(),
            StripePaymentIntentId = stripePaymentIntentId?.Trim(),
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero),
            Status = PaymentAttemptStatus.Created,
            CreatedAt = nowUtc,
            UpdatedAt = null,
            FailureCode = null
        };
    }

    public void MarkAsSucceeded(string stripePaymentIntentId, DateTime nowUtc)
    {
        EnsureCreated();

        if (string.IsNullOrWhiteSpace(stripePaymentIntentId))
            throw new DomainException("StripePaymentIntentId is required when marking payment as succeeded.");

        Status = PaymentAttemptStatus.Succeeded;
        StripePaymentIntentId = stripePaymentIntentId.Trim();
        UpdatedAt = nowUtc;
        FailureCode = null;
    }

    public void MarkAsFailed(string failureCode, DateTime nowUtc)
    {
        EnsureCreated();

        if (string.IsNullOrWhiteSpace(failureCode))
            throw new DomainException("FailureCode is required when marking payment as failed.");

        Status = PaymentAttemptStatus.Failed;
        FailureCode = failureCode.Trim();
        UpdatedAt = nowUtc;
    }

    public void MarkAsExpired(DateTime nowUtc)
    {
        EnsureCreated();

        Status = PaymentAttemptStatus.Expired;
        UpdatedAt = nowUtc;
    }

    private void EnsureCreated()
    {
        if (Status != PaymentAttemptStatus.Created)
            throw new DomainException($"Cannot change status of a payment attempt that is already '{Status}'.");
    }
}

