using DonationHub.Domain.Enums;
using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class Donation
{
    public const decimal DefaultMinAmount = 1.00m;
    public const decimal DefaultMaxAmount = 10000.00m;

    public Guid Id { get; private set; }
    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }
    public decimal Amount { get; private set; }
    public DonationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public string? ReceiptNumber { get; private set; }
    public string? ReceiptFileName { get; private set; }

    private Donation()
    {
    }

    public static Donation Create(
        Guid id,
        Guid campaignId,
        Guid donorUserId,
        decimal amount,
        DateTime nowUtc,
        decimal minAmount = DefaultMinAmount,
        decimal maxAmount = DefaultMaxAmount)
    {
        if (id == Guid.Empty)
            throw new DomainException("Donation Id cannot be empty.");

        if (campaignId == Guid.Empty)
            throw new DomainException("CampaignId cannot be empty.");

        if (donorUserId == Guid.Empty)
            throw new DomainException("DonorUserId cannot be empty.");

        // Domain Invariant 4: مبلغ التبرع يقع بين الحد الأدنى والأقصى
        if (amount < minAmount || amount > maxAmount)
            throw new DomainException($"Donation amount ({amount:0.00}) must be between {minAmount:0.00} and {maxAmount:0.00}.");

        return new Donation
        {
            Id = id,
            CampaignId = campaignId,
            DonorUserId = donorUserId,
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero),
            Status = DonationStatus.Pending,
            CreatedAt = nowUtc,
            CompletedAt = null,
            FailureReason = null,
            ReceiptNumber = null,
            ReceiptFileName = null
        };
    }

    /// <summary>
    /// Domain Invariant 8: تأكيد نجاح التبرع (الانتقال إلى حالة نهائية لا تتغير)
    /// </summary>
    public void MarkAsSucceeded(string receiptNumber, DateTime nowUtc, string? receiptFileName = null)
    {
        EnsurePending();

        if (string.IsNullOrWhiteSpace(receiptNumber))
            throw new DomainException("Receipt number is required when marking donation as succeeded.");

        Status = DonationStatus.Succeeded;
        CompletedAt = nowUtc;
        ReceiptNumber = receiptNumber.Trim();
        ReceiptFileName = receiptFileName?.Trim();
        FailureReason = null;
    }

    /// <summary>
    /// Domain Invariant 8: تسجيل فشل التبرع (الانتقال إلى حالة نهائية لا تتغير)
    /// </summary>
    public void MarkAsFailed(string failureReason, DateTime nowUtc)
    {
        EnsurePending();

        if (string.IsNullOrWhiteSpace(failureReason))
            throw new DomainException("Failure reason is required when marking donation as failed.");

        Status = DonationStatus.Failed;
        CompletedAt = nowUtc;
        FailureReason = failureReason.Trim();
    }

    private void EnsurePending()
    {
        // Domain Invariant 8: الحالات النهائية (Succeeded, Failed) غير قابلة للتغيير
        if (Status != DonationStatus.Pending)
            throw new DomainException($"Cannot modify a finished donation with status '{Status}'. Final states are immutable.");
    }
}

