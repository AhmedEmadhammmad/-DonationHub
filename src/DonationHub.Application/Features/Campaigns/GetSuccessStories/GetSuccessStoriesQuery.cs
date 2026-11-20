using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.GetSuccessStories;

/// <summary>
/// Query retrieving public list of successfully completed campaigns (CMP-04, CMP-11).
/// </summary>
public sealed record GetSuccessStoriesQuery(string? AcceptLanguage = null) : IRequest<Result<IReadOnlyList<CampaignPublicDto>>>;
