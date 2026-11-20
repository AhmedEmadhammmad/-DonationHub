using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.GetCategories;

/// <summary>
/// Handles all-category retrieval using high-throughput AsNoTracking read path (CAT-03).
/// </summary>
public sealed class GetCategoriesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.NameEn)
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
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CategoryDto>>(categories);
    }
}
