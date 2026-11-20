using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Categories.UpdateCategory;

/// <summary>
/// Command requesting updates to an existing category (CAT-01, CAT-02).
/// </summary>
public sealed record UpdateCategoryCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn) : IRequest<Result<CategoryDto>>;
