using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Campaigns.GetActiveCampaigns;

/// <summary>
/// Handles retrieving active campaigns via direct AsNoTracking read path with language localization (CMP-04, CMP-11).
/// </summary>
public sealed class GetActiveCampaignsQueryHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<GetActiveCampaignsQuery, Result<IReadOnlyList<CampaignPublicDto>>>
{
    public async Task<Result<IReadOnlyList<CampaignPublicDto>>> Handle(GetActiveCampaignsQuery request, CancellationToken cancellationToken)
    {
        var isArabic = request.AcceptLanguage?.Trim().StartsWith("ar", StringComparison.OrdinalIgnoreCase) == true;
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var campaigns = await context.Campaigns
            .AsNoTracking()
            .Where(c => c.Status == CampaignStatus.Active)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                Title = isArabic ? c.TitleAr : c.TitleEn,
                Description = isArabic ? c.DescriptionAr : c.DescriptionEn,
                c.GoalAmount,
                c.RaisedAmount,
                c.EndDate,
                c.CategoryId,
                Status = c.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        var result = campaigns.Select(c => new CampaignPublicDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            GoalAmount = c.GoalAmount,
            RaisedAmount = c.RaisedAmount,
            ProgressPercent = c.GoalAmount > 0
                ? Math.Min(100m, Math.Round((c.RaisedAmount / c.GoalAmount) * 100, 2))
                : 0m,
            DaysLeft = Math.Max(0, (int)Math.Ceiling((c.EndDate - nowUtc).TotalDays)),
            EndDate = c.EndDate,
            CategoryId = c.CategoryId,
            Status = c.Status
        }).ToList();

        return Result.Success<IReadOnlyList<CampaignPublicDto>>(result);
    }
}
