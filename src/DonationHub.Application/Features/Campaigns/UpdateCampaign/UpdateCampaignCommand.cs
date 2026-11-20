using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.UpdateCampaign;

/// <summary>
/// Command requesting updates to an active campaign, including concurrency token verification (CMP-02, CMP-10).
/// </summary>
public sealed record UpdateCampaignCommand(
    Guid Id,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    decimal GoalAmount,
    DateTime EndDate,
    Guid CategoryId,
    string RowVersion) : IRequest<Result<CampaignAdminDto>>;
