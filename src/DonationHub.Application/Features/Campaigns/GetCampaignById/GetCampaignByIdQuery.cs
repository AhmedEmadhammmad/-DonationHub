using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.GetCampaignById;

/// <summary>
/// Query retrieving a single campaign's details with computed progress metrics (CMP-05, CMP-11).
/// </summary>
public sealed record GetCampaignByIdQuery(Guid Id, string? AcceptLanguage = null) : IRequest<Result<CampaignPublicDto>>;
