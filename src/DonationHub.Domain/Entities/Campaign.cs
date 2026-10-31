using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class Campaign : ISoftDeletable
{
    public Guid Id { get; private set; }
    public string TitleAr { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;
    public decimal GoalAmount { get; private set; }
    public decimal RaisedAmount { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid CategoryId { get; private set; }
    public string? BannerFileName { get; private set; }
    public CampaignStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private Campaign()
    {
    }

    public static Campaign Create(
        Guid id,
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        decimal goalAmount,
        DateTime endDateUtc,
        Guid categoryId,
        Guid createdByUserId,
        DateTime nowUtc,
        string? bannerFileName = null)
    {
        if (id == Guid.Empty)
            throw new DomainException("Campaign Id cannot be empty.");

        if (categoryId == Guid.Empty)
            throw new DomainException("CategoryId cannot be empty.");

        if (createdByUserId == Guid.Empty)
            throw new DomainException("CreatedByUserId cannot be empty.");

        ValidateText(titleAr, titleEn, descriptionAr, descriptionEn);

        // Domain Invariant 1: GoalAmount > 0
        if (goalAmount <= 0)
            throw new DomainException("Campaign goal amount must be strictly greater than zero.");

        // Domain Invariant 2: EndDate > nowUtc
        if (endDateUtc <= nowUtc)
            throw new DomainException("Campaign end date must be in the future.");

        return new Campaign
        {
            Id = id,
            TitleAr = titleAr.Trim(),
            TitleEn = titleEn.Trim(),
            DescriptionAr = descriptionAr.Trim(),
            DescriptionEn = descriptionEn.Trim(),
            GoalAmount = decimal.Round(goalAmount, 2, MidpointRounding.AwayFromZero),
            RaisedAmount = 0.00m,
            EndDate = endDateUtc,
            CategoryId = categoryId,
            CreatedByUserId = createdByUserId,
            BannerFileName = bannerFileName?.Trim(),
            Status = CampaignStatus.Active,
            CreatedAt = nowUtc,
            UpdatedAt = null,
            IsDeleted = false,
            DeletedAtUtc = null
        };
    }

    public void UpdateDetails(
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        string? bannerFileName,
        DateTime nowUtc)
    {
        EnsureNotFinished();
        ValidateText(titleAr, titleEn, descriptionAr, descriptionEn);

        TitleAr = titleAr.Trim();
        TitleEn = titleEn.Trim();
        DescriptionAr = descriptionAr.Trim();
        DescriptionEn = descriptionEn.Trim();
        BannerFileName = bannerFileName?.Trim();
        UpdatedAt = nowUtc;
    }


    public void EnsureCanAcceptDonation(DateTime nowUtc)
    {
        if (Status != CampaignStatus.Active)
            throw new DomainException($"Cannot accept donations on a campaign with status '{Status}'. Only active campaigns accept donations.");

        if (EndDate <= nowUtc)
            throw new DomainException("Cannot accept donations on an expired campaign.");
    }


    public void RegisterSuccessfulDonation(decimal donationAmount, DateTime nowUtc)
    {
        EnsureCanAcceptDonation(nowUtc);

        if (donationAmount <= 0)
            throw new DomainException("Donation amount must be strictly greater than zero.");

        RaisedAmount = decimal.Round(RaisedAmount + donationAmount, 2, MidpointRounding.AwayFromZero);
        UpdatedAt = nowUtc;

        // Invariant 6: Auto-complete campaign when goal is reached or exceeded
        if (RaisedAmount >= GoalAmount)
        {
            Status = CampaignStatus.Completed;
        }
    }


    public void UpdateGoalAmount(decimal newGoalAmount, DateTime nowUtc)
    {
        EnsureNotFinished();

        if (newGoalAmount <= 0)
            throw new DomainException("Campaign goal amount must be strictly greater than zero.");

        if (newGoalAmount < RaisedAmount)
            throw new DomainException($"Cannot lower GoalAmount ({newGoalAmount:0.00}) below already RaisedAmount ({RaisedAmount:0.00}).");

        GoalAmount = decimal.Round(newGoalAmount, 2, MidpointRounding.AwayFromZero);
        UpdatedAt = nowUtc;

        // If goal is now <= raised amount, complete it
        if (RaisedAmount >= GoalAmount)
        {
            Status = CampaignStatus.Completed;
        }
    }


    public void ExtendEndDate(DateTime newEndDateUtc, DateTime nowUtc)
    {
        EnsureNotFinished();

        if (newEndDateUtc <= nowUtc)
            throw new DomainException("New end date must be strictly in the future.");

        if (newEndDateUtc <= EndDate)
            throw new DomainException("New end date must be greater than current end date.");

        EndDate = newEndDateUtc;
        UpdatedAt = nowUtc;
    }

    public void Expire(DateTime nowUtc)
    {
        if (Status == CampaignStatus.Completed)
            throw new DomainException("Cannot expire an already completed campaign.");

        if (nowUtc < EndDate)
            throw new DomainException("Cannot expire a campaign before its end date has passed.");

        Status = CampaignStatus.Expired;
        UpdatedAt = nowUtc;
    }


    public void EnsureCanBePhysicallyDeleted(bool hasDonations)
    {
        if (hasDonations)
            throw new DomainException("Campaign with existing donations cannot be physically deleted; soft delete must be used.");
    }

    private void EnsureNotFinished()
    {
        if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired)
            throw new DomainException($"Cannot modify a finished campaign with status '{Status}'.");
    }

    private static void ValidateText(string titleAr, string titleEn, string descriptionAr, string descriptionEn)
    {
        if (string.IsNullOrWhiteSpace(titleAr))
            throw new DomainException("Arabic title cannot be empty.");

        if (string.IsNullOrWhiteSpace(titleEn))
            throw new DomainException("English title cannot be empty.");

        if (string.IsNullOrWhiteSpace(descriptionAr))
            throw new DomainException("Arabic description cannot be empty.");

        if (string.IsNullOrWhiteSpace(descriptionEn))
            throw new DomainException("English description cannot be empty.");
    }
}

