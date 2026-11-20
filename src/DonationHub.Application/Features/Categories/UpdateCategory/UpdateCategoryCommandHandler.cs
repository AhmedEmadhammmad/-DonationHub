using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.UpdateCategory;

/// <summary>
/// Handles category updates with duplicate name prevention and persistence via Unit of Work (CAT-01).
/// </summary>
public sealed class UpdateCategoryCommandHandler(
    IGenericRepository<Category> repository,
    IAppDbContext context,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.NotFound);
        }

        var trimmedNameAr = request.NameAr.Trim();
        var trimmedNameEn = request.NameEn.Trim();

        var isDuplicate = await context.Categories
            .AnyAsync(c => c.Id != request.Id && (c.NameAr == trimmedNameAr || c.NameEn == trimmedNameEn), cancellationToken);

        if (isDuplicate)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.DuplicateName);
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        category.Update(
            trimmedNameAr,
            trimmedNameEn,
            request.DescriptionAr,
            request.DescriptionEn,
            nowUtc);

        repository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new CategoryDto
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        return Result.Success(dto);
    }
}
