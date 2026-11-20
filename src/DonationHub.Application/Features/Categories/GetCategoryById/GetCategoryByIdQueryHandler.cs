using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.GetCategoryById;

/// <summary>
/// Handles single category retrieval using direct AsNoTracking and projection (CAT-03).
/// </summary>
public sealed class GetCategoryByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn,
                DescriptionAr = c.DescriptionAr,
                DescriptionEn = c.DescriptionEn,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return Result.Failure<CategoryDto>(CategoryErrors.NotFound);
        }

        return Result.Success(category);
    }
}
