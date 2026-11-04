using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.Entities;

/// <summary>
/// Domain entity representing a platform user with authentication credentials and assigned roles.
/// </summary>
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
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = RoleDonor;
    public string PreferredLanguage { get; private set; } = LanguageEn;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Parameterless constructor required for EF Core entity materialization.
    /// </summary>
    private ApplicationUser()
    {
    }

    /// <summary>
    /// Factory method creating a new ApplicationUser ensuring all business invariants are validated.
    /// </summary>
    public static ApplicationUser Create(
        Guid id,
        string email,
        string userName,
        string fullName,
        string passwordHash,
        DateTime nowUtc,
        string role = RoleDonor,
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

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("PasswordHash cannot be empty.");

        if (role != RoleAdmin && role != RoleCampaignManager && role != RoleDonor)
            throw new DomainException($"Invalid user role: {role}");

        var normalizedLanguage = NormalizeLanguage(preferredLanguage);

        return new ApplicationUser
        {
            Id = id,
            Email = email.Trim().ToLowerInvariant(),
            UserName = userName.Trim(),
            FullName = fullName.Trim(),
            PasswordHash = passwordHash,
            Role = role,
            PreferredLanguage = normalizedLanguage,
            IsActive = isActive,
            CreatedAt = nowUtc
        };
    }

    /// <summary>
    /// Updates user password hash when changing or resetting credentials.
    /// </summary>
    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("PasswordHash cannot be empty.");

        PasswordHash = newPasswordHash;
    }

    /// <summary>
    /// Updates user role for administrative promotions.
    /// </summary>
    public void UpdateRole(string newRole)
    {
        if (newRole != RoleAdmin && newRole != RoleCampaignManager && newRole != RoleDonor)
            throw new DomainException($"Invalid user role: {newRole}");

        Role = newRole;
    }

    /// <summary>
    /// Deactivates user account preventing login and token renewal.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Reactivates user account enabling full platform access.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Updates basic user profile information.
    /// </summary>
    public void UpdateProfile(string fullName, string preferredLanguage)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("FullName cannot be empty.");

        FullName = fullName.Trim();
        PreferredLanguage = NormalizeLanguage(preferredLanguage);
    }

    /// <summary>
    /// Normalizes language input to either English ('en') or Arabic ('ar').
    /// </summary>
    private static string NormalizeLanguage(string lang)
    {
        var cleaned = (lang ?? string.Empty).Trim().ToLowerInvariant();
        if (cleaned != LanguageEn && cleaned != LanguageAr)
            return LanguageEn;

        return cleaned;
    }
}
