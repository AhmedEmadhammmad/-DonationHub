using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.DeleteCampaign;

/// <summary>
/// Command requesting soft deletion of an active campaign (CMP-03).
/// </summary>
public sealed record DeleteCampaignCommand(Guid Id) : IRequest<Result>;
