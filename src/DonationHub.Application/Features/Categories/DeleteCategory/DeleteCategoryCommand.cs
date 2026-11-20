using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Categories.DeleteCategory;

/// <summary>
/// Command requesting the deletion of an existing category (CAT-01).
/// </summary>
public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
