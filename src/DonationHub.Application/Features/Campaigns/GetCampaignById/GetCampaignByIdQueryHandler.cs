using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Campaigns.GetCampaignById;

/// <summary>
/// Handles retrieving individual campaign with localized fields and computed metrics (CMP-05, CMP-11).
/// </summary>
public sealed class GetCampaignByIdQueryHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<GetCampaignByIdQuery, Result<CampaignPublicDto>>
{
    public async Task<Result<CampaignPublicDto>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
    {
        var isArabic = request.AcceptLanguage?.Trim().StartsWith("ar", StringComparison.OrdinalIgnoreCase) == true;
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var campaign = await context.Campaigns
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (campaign is null)
        {
            return Result.Failure<CampaignPublicDto>(CampaignErrors.NotFound);
        }

        var dto = new CampaignPublicDto
        {
            Id = campaign.Id,
            Title = campaign.Title,
            Description = campaign.Description,
            GoalAmount = campaign.GoalAmount,
            RaisedAmount = campaign.RaisedAmount,
            ProgressPercent = campaign.GoalAmount > 0
                ? Math.Min(100m, Math.Round((campaign.RaisedAmount / campaign.GoalAmount) * 100, 2))
                : 0m,
            DaysLeft = Math.Max(0, (int)Math.Ceiling((campaign.EndDate - nowUtc).TotalDays)),
            EndDate = campaign.EndDate,
            CategoryId = campaign.CategoryId,
            Status = campaign.Status
        };

        return Result.Success(dto);
    }
}
