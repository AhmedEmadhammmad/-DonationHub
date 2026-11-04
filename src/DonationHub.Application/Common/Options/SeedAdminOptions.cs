using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Common.Options;

/// <summary>
/// Strongly-typed configuration options for seeding the initial platform administrator account at startup.
/// </summary>
public class SeedAdminOptions
{
    /// <summary>
    /// Configuration section key within configuration files.
    /// </summary>
    public const string SectionName = "SeedAdmin";

    /// <summary>
    /// Initial administrator email address.
    /// </summary>
    [Required(ErrorMessage = "Seed Admin Email is required.")]
    [EmailAddress(ErrorMessage = "Seed Admin Email must be a valid email address.")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Initial administrator password meeting complexity requirements.
    /// </summary>
    [Required(ErrorMessage = "Seed Admin Password is required.")]
    [MinLength(8, ErrorMessage = "Seed Admin Password must be at least 8 characters long.")]
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Initial administrator full name display value.
    /// </summary>
    [Required(ErrorMessage = "Seed Admin FullName is required.")]
    public string FullName { get; init; } = string.Empty;
}

