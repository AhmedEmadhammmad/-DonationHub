using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Categories.GetCategories;

/// <summary>
/// Query retrieving all categories for public listing (CAT-03).
/// </summary>
public sealed record GetCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryDto>>>;
