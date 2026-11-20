using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.GetActiveCampaigns;

/// <summary>
/// Query retrieving public list of active campaigns localized per Accept-Language header (CMP-04, CMP-11).
/// </summary>
public sealed record GetActiveCampaignsQuery(string? AcceptLanguage = null) : IRequest<Result<IReadOnlyList<CampaignPublicDto>>>;
