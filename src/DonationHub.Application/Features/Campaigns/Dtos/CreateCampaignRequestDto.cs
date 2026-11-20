namespace DonationHub.Application.Features.Campaigns.Dtos;

/// <summary>
/// HTTP request payload for creating a new crowdfunding campaign (CMP-01).
/// </summary>
public class CreateCampaignRequestDto
{
    public string TitleAr { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string DescriptionAr { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public decimal GoalAmount { get; init; }
    public DateTime EndDate { get; init; }
    public Guid CategoryId { get; init; }
}
