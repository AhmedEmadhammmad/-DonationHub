namespace DonationHub.Application.Features.Campaigns.Dtos;

/// <summary>
/// Data transfer object exposing full bilingual campaign details and concurrency token for administrative management.
/// </summary>
public class CampaignAdminDto
{
    public Guid Id { get; init; }
    public string TitleAr { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string DescriptionAr { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public decimal GoalAmount { get; init; }
    public decimal RaisedAmount { get; init; }
    public DateTime EndDate { get; init; }
    public Guid CategoryId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string RowVersion { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
