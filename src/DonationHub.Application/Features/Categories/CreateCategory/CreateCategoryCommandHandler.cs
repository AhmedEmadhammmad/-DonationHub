using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.CreateCategory;

/// <summary>
/// Handles category creation via repository and unit of work writes (CAT-01, CAT-02).
/// </summary>
public sealed class CreateCategoryCommandHandler(
    IGenericRepository<Category> repository,
    IAppDbContext context,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var trimmedNameAr = request.NameAr.Trim();
        var trimmedNameEn = request.NameEn.Trim();

        var isDuplicate = await context.Categories
            .AnyAsync(c => c.NameAr == trimmedNameAr || c.NameEn == trimmedNameEn, cancellationToken);

        if (isDuplicate)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.DuplicateName);
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var category = Category.Create(
            Guid.NewGuid(),
            trimmedNameAr,
            trimmedNameEn,
            nowUtc,
            request.DescriptionAr,
            request.DescriptionEn);

        await repository.AddAsync(category, cancellationToken);
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
