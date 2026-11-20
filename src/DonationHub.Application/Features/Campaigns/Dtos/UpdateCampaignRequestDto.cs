namespace DonationHub.Application.Features.Campaigns.Dtos;

/// <summary>
/// HTTP request payload for updating campaign details including concurrency token (CMP-02, CMP-10).
/// </summary>
public class UpdateCampaignRequestDto
{
    public string TitleAr { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string DescriptionAr { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public decimal GoalAmount { get; init; }
    public DateTime EndDate { get; init; }
    public Guid CategoryId { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
