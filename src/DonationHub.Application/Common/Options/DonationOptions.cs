using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Common.Options;

/// <summary>
/// Strongly-typed configuration options governing donation thresholds, currencies, and timeout durations.
/// </summary>
public class DonationOptions
{
    /// <summary>
    /// Configuration section key within configuration files.
    /// </summary>
    public const string SectionName = "Donations";

    /// <summary>
    /// Minimum allowable single donation amount.
    /// </summary>
    [Range(typeof(decimal), "0.01", "1000000.00", ErrorMessage = "MinAmount must be at least 0.01.")]
    public decimal MinAmount { get; init; } = 1.00m;

    /// <summary>
    /// Maximum allowable single donation amount.
    /// </summary>
    [Range(typeof(decimal), "1.00", "1000000.00", ErrorMessage = "MaxAmount must be greater than or equal to 1.00.")]
    public decimal MaxAmount { get; init; } = 10000.00m;

    /// <summary>
    /// Duration in minutes before an uncompleted donation payment attempt expires.
    /// </summary>
    [Range(1, 1440, ErrorMessage = "PendingTimeoutMinutes must be between 1 and 1440 minutes.")]
    public int PendingTimeoutMinutes { get; init; } = 30;

    /// <summary>
    /// Default ISO 4217 three-letter currency code for platform transactions.
    /// </summary>
    [Required(ErrorMessage = "DefaultCurrency is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "DefaultCurrency must be a 3-letter currency code.")]
    public string DefaultCurrency { get; init; } = "USD";
}

