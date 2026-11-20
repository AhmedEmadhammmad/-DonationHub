using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.DeleteCategory;

/// <summary>
/// Handles category deletion, enforcing Invariant 10: rejecting deletion when linked campaigns exist (CAT-04).
/// </summary>
public sealed class DeleteCategoryCommandHandler(
    IGenericRepository<Category> repository,
    IAppDbContext context,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }

        // CAT-04 / Invariant 10: Reject with 409 Conflict if category has active or completed campaigns
        var hasCampaigns = await context.Campaigns
            .AnyAsync(c => c.CategoryId == request.Id, cancellationToken);

        if (hasCampaigns)
        {
            return Result.Failure(CategoryErrors.HasCampaigns);
        }

        repository.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
