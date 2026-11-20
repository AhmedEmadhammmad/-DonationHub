using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.DeleteCampaign;

/// <summary>
/// Handles campaign soft deletion through repository, ensuring it never appears in queries (CMP-03).
/// </summary>
public sealed class DeleteCampaignCommandHandler(
    IGenericRepository<Campaign> repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCampaignCommand, Result>
{
    public async Task<Result> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
        {
            return Result.Failure(CampaignErrors.NotFound);
        }

        // Soft deletion enforced via SoftDeleteInterceptor
        repository.Delete(campaign);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
