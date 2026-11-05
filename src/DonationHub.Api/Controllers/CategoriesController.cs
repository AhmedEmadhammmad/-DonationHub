using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Application.Features.Categories.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Api.Controllers;

/// <summary>
/// Exposes endpoints for managing and retrieving campaign categories.
/// </summary>
[ApiController]
[Route("api/v1/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Initializes CategoriesController with the category application service.
    /// </summary>
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

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
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _categoryService.CreateCategoryAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }
        catch (CategoryDuplicateNameException ex)
        {
            return Conflict(new
            {
                error = CategoryDuplicateNameException.ErrorCode,
                message = ex.Message
            });
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Conflict(new
            {
                error = CategoryDuplicateNameException.ErrorCode,
                message = "A category with the same Arabic or English name already exists."
            });
        }
    }

    /// <summary>
    /// Retrieves all active categories for public browsing (CAT-03).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoriesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single category by identifier (CAT-03).
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (CategoryNotFoundException ex)
        {
            return NotFound(new
            {
                error = CategoryNotFoundException.ErrorCode,
                message = ex.Message
            });
        }
    }

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
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request, cancellationToken);
            return Ok(result);
        }
        catch (CategoryNotFoundException ex)
        {
            return NotFound(new
            {
                error = CategoryNotFoundException.ErrorCode,
                message = ex.Message
            });
        }
        catch (CategoryDuplicateNameException ex)
        {
            return Conflict(new
            {
                error = CategoryDuplicateNameException.ErrorCode,
                message = ex.Message
            });
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Conflict(new
            {
                error = CategoryDuplicateNameException.ErrorCode,
                message = "A category with the same Arabic or English name already exists."
            });
        }
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
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(id, cancellationToken);
            return NoContent();
        }
        catch (CategoryNotFoundException ex)
        {
            return NotFound(new
            {
                error = CategoryNotFoundException.ErrorCode,
                message = ex.Message
            });
        }
        catch (CategoryHasCampaignsException ex)
        {
            return Conflict(new
            {
                error = CategoryHasCampaignsException.ErrorCode,
                message = ex.Message
            });
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is SqlException { Number: 2601 or 2627 };
}

