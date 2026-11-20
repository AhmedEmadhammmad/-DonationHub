using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Campaigns.UpdateCampaign;

/// <summary>
/// Handles campaign modification, enforcing Invariants 2, 7, 8 and catching optimistic concurrency conflicts (CMP-02, CMP-10).
/// </summary>
public sealed class UpdateCampaignCommandHandler(
    IGenericRepository<Campaign> repository,
    IAppDbContext context,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<UpdateCampaignCommand, Result<CampaignAdminDto>>
{
    public async Task<Result<CampaignAdminDto>> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
        {
            return Result.Failure<CampaignAdminDto>(CampaignErrors.NotFound);
        }

        // Invariant 8: Final state cannot change
        if (campaign.Status is CampaignStatus.Completed or CampaignStatus.Expired)
        {
            return Result.Failure<CampaignAdminDto>(CampaignErrors.NotActive);
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        // Invariant 2: End date in future
        if (request.EndDate <= nowUtc)
        {
            return Result.Failure<CampaignAdminDto>(CampaignErrors.EndDateInPast);
        }

        // Invariant 7: Goal not below raised
        if (request.GoalAmount < campaign.RaisedAmount)
        {
            return Result.Failure<CampaignAdminDto>(CampaignErrors.GoalBelowRaised);
        }

        var categoryExists = await context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            return Result.Failure<CampaignAdminDto>(CategoryErrors.NotFound);
        }

        campaign.Update(
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.GoalAmount,
            request.EndDate,
            request.CategoryId,
            nowUtc);

        // CMP-10: Bind incoming RowVersion as original value for concurrency detection
        if (!string.IsNullOrWhiteSpace(request.RowVersion))
        {
            try
            {
                var originalBytes = Convert.FromBase64String(request.RowVersion);
                context.Entry(campaign).Property(c => c.RowVersion).OriginalValue = originalBytes;
            }
            catch (FormatException)
            {
                return Result.Failure<CampaignAdminDto>(CampaignErrors.ConcurrencyConflict);
            }
        }

        repository.Update(campaign);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Concurrency conflict caught -> map to 409 Conflict per CMP-10
            return Result.Failure<CampaignAdminDto>(CampaignErrors.ConcurrencyConflict);
        }

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
