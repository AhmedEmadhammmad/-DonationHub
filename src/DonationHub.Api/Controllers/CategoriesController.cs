using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Categories.CreateCategory;
using DonationHub.Application.Features.Categories.DeleteCategory;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Application.Features.Categories.GetCategories;
using DonationHub.Application.Features.Categories.GetCategoryById;
using DonationHub.Application.Features.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

/// <summary>
/// Exposes endpoints for managing and retrieving campaign categories via CQRS slices.
/// </summary>
[ApiController]
[Route("api/v1/categories")]
public class CategoriesController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new category under administrator authorization (CAT-01 / CAT-02).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedAtActionResult(
            this,
            nameof(GetCategoryById),
            new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    /// <summary>
    /// Retrieves all active categories for public browsing (CAT-03).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken) =>
        (await mediator.Send(new GetCategoriesQuery(), cancellationToken)).ToActionResult(this);

    /// <summary>
    /// Retrieves a single category by identifier (CAT-03).
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetCategoryById))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken) =>
        (await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken)).ToActionResult(this);

    /// <summary>
    /// Updates category details under administrator authorization (CAT-01 / CAT-02).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.NameAr,
            request.NameEn,
            request.DescriptionAr,
            request.DescriptionEn);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Deletes a category ensuring it is not referenced by any campaigns (CAT-01 / CAT-04).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken) =>
        (await mediator.Send(new DeleteCategoryCommand(id), cancellationToken)).ToActionResult(this);
}
