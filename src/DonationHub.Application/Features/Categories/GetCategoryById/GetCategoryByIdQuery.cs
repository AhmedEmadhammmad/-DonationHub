using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Categories.GetCategoryById;

/// <summary>
/// Query retrieving a single category by its unique identifier (CAT-03).
/// </summary>
public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;
