using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Campaigns.CreateCampaign;

/// <summary>
/// Handles campaign creation, enforcing Category existence and future end date (CMP-01).
/// </summary>
public sealed class CreateCampaignCommandHandler(
    IGenericRepository<Campaign> repository,
    IAppDbContext context,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CreateCampaignCommand, Result<CampaignAdminDto>>
{
    public async Task<Result<CampaignAdminDto>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            return Result.Failure<CampaignAdminDto>(CategoryErrors.NotFound);
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        if (request.EndDate <= nowUtc)
        {
            return Result.Failure<CampaignAdminDto>(CampaignErrors.EndDateInPast);
        }

        var campaign = Campaign.Create(
            Guid.NewGuid(),
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.GoalAmount,
            request.EndDate,
            request.CategoryId,
            request.CreatedByUserId,
            nowUtc);

        await repository.AddAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new CampaignAdminDto
        {
            Id = campaign.Id,
            TitleAr = campaign.TitleAr,
            TitleEn = campaign.TitleEn,
            DescriptionAr = campaign.DescriptionAr,
            DescriptionEn = campaign.DescriptionEn,
            GoalAmount = campaign.GoalAmount,
            RaisedAmount = campaign.RaisedAmount,
            EndDate = campaign.EndDate,
            CategoryId = campaign.CategoryId,
            Status = campaign.Status.ToString(),
            RowVersion = Convert.ToBase64String(campaign.RowVersion),
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt
        };

        return Result.Success(dto);
    }
}
