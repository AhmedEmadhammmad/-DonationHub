using DonationHub.Application.Features.Categories.Dtos;

namespace DonationHub.Application.Features.Categories.Services;

/// <summary>
/// Service interface defining category lifecycle operations and business validation rules.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Creates a new bilingual category under administrator authorization (CAT-01 / CAT-02).
    /// </summary>
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active categories projected directly to DTOs for public consumption (CAT-03).
    /// </summary>
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single category by identifier (CAT-03).
    /// </summary>
    Task<CategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category's bilingual names and descriptions (CAT-01 / CAT-02).
    /// </summary>
    Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category ensuring no associated campaigns exist (CAT-01 / CAT-04).
    /// </summary>
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}

