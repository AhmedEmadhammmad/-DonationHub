namespace DonationHub.Application.Features.Campaigns.Dtos;

/// <summary>
/// Data transfer object exposing localized campaign information and progress calculations for public consumers (CMP-05, CMP-11).
/// </summary>
public class CampaignPublicDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal GoalAmount { get; init; }
    public decimal RaisedAmount { get; init; }
    public decimal ProgressPercent { get; init; }
    public int DaysLeft { get; init; }
    public DateTime EndDate { get; init; }
    public Guid CategoryId { get; init; }
    public string Status { get; init; } = string.Empty;
}
