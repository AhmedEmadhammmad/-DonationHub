using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

public class ApplicationUser
{
    public const string RoleAdmin = "Admin";
    public const string RoleCampaignManager = "CampaignManager";
    public const string RoleDonor = "Donor";

    public const string LanguageEn = "en";
    public const string LanguageAr = "ar";

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PreferredLanguage { get; private set; } = LanguageEn;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private ApplicationUser()
    {
    }

    public static ApplicationUser Create(
        Guid id,
        string email,
        string userName,
        string fullName,
        DateTime nowUtc,
        string preferredLanguage = LanguageEn,
        bool isActive = true)
    {
        if (id == Guid.Empty)
            throw new DomainException("User Id cannot be empty.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("UserName cannot be empty.");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("FullName cannot be empty.");

        var normalizedLanguage = NormalizeLanguage(preferredLanguage);

        return new ApplicationUser
        {
            Id = id,
            Email = email.Trim().ToLowerInvariant(),
            UserName = userName.Trim(),
            FullName = fullName.Trim(),
            PreferredLanguage = normalizedLanguage,
            IsActive = isActive,
            CreatedAt = nowUtc
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateProfile(string fullName, string preferredLanguage)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("FullName cannot be empty.");

        FullName = fullName.Trim();
        PreferredLanguage = NormalizeLanguage(preferredLanguage);
    }

    private static string NormalizeLanguage(string lang)
    {
        var cleaned = (lang ?? string.Empty).Trim().ToLowerInvariant();
        if (cleaned != LanguageEn && cleaned != LanguageAr)
            return LanguageEn;

        return cleaned;
    }
}

