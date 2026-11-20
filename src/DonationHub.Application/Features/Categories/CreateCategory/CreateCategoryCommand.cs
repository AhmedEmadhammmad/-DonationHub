using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Categories.CreateCategory;

/// <summary>
/// Command requesting the creation of a new bilingual category.
/// </summary>
public sealed record CreateCategoryCommand(
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn) : IRequest<Result<CategoryDto>>;
