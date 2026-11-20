using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.CreateCampaign;

/// <summary>
/// Command requesting creation of a new crowdfunding campaign (CMP-01).
/// </summary>
public sealed record CreateCampaignCommand(
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    decimal GoalAmount,
    DateTime EndDate,
    Guid CategoryId,
    Guid CreatedByUserId) : IRequest<Result<CampaignAdminDto>>;
