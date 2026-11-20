using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Campaigns.GetSuccessStories;

/// <summary>
/// Handles retrieving completed campaigns for public success stories via direct AsNoTracking (CMP-04).
/// </summary>
public sealed class GetSuccessStoriesQueryHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<GetSuccessStoriesQuery, Result<IReadOnlyList<CampaignPublicDto>>>
{
    public async Task<Result<IReadOnlyList<CampaignPublicDto>>> Handle(GetSuccessStoriesQuery request, CancellationToken cancellationToken)
    {
        var isArabic = request.AcceptLanguage?.Trim().StartsWith("ar", StringComparison.OrdinalIgnoreCase) == true;
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var campaigns = await context.Campaigns
            .AsNoTracking()
            .Where(c => c.Status == CampaignStatus.Completed)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
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
            ProgressPercent = 100m,
            DaysLeft = 0,
            EndDate = c.EndDate,
            CategoryId = c.CategoryId,
            Status = c.Status
        }).ToList();

        return Result.Success<IReadOnlyList<CampaignPublicDto>>(result);
    }
}
