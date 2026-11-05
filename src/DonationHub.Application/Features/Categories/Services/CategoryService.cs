using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Categories.Dtos;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.Services;

/// <summary>
/// Implements category workflows including creation, retrieval, updates, and invariant-checked deletion.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IAppDbContext _context;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes CategoryService with database context, generic repository, and unit of work.
    /// </summary>
    public CategoryService(
        IAppDbContext context,
        IGenericRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Creates a new category enforcing bilingual uniqueness and domain rules (CAT-01 / CAT-02).
    /// </summary>
    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var trimmedNameAr = request.NameAr.Trim();
        var trimmedNameEn = request.NameEn.Trim();

        // 1. Enforce unique Arabic name
        var arabicNameExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.NameAr == trimmedNameAr, cancellationToken);

        if (arabicNameExists)
        {
            throw new CategoryDuplicateNameException(trimmedNameAr, "Arabic");
        }

        // 2. Enforce unique English name
        var englishNameExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.NameEn == trimmedNameEn, cancellationToken);

        if (englishNameExists)
        {
            throw new CategoryDuplicateNameException(trimmedNameEn, "English");
        }

        // 3. Create encapsulated domain entity with validation
        var category = Category.Create(
            id: Guid.NewGuid(),
            nameAr: trimmedNameAr,
            nameEn: trimmedNameEn,
            nowUtc: DateTime.UtcNow,
            descriptionAr: request.DescriptionAr,
            descriptionEn: request.DescriptionEn);

        // 4. Persistence via repository and unit of work
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    /// <summary>
    /// Retrieves all categories projected directly into DTOs using AsNoTracking for high performance (CAT-03).
    /// </summary>
    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.NameAr)
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
    }

    /// <summary>
    /// Retrieves a single category by identifier (CAT-03).
    /// </summary>
    public async Task<CategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        return MapToDto(category);
    }

    /// <summary>
    /// Updates category names and descriptions enforcing uniqueness (CAT-01 / CAT-02).
    /// </summary>
    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        var trimmedNameAr = request.NameAr.Trim();
        var trimmedNameEn = request.NameEn.Trim();

        if (!string.Equals(category.NameAr, trimmedNameAr, StringComparison.OrdinalIgnoreCase))
        {
            var arabicExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id != id && c.NameAr == trimmedNameAr, cancellationToken);

            if (arabicExists)
            {
                throw new CategoryDuplicateNameException(trimmedNameAr, "Arabic");
            }
        }

        if (!string.Equals(category.NameEn, trimmedNameEn, StringComparison.OrdinalIgnoreCase))
        {
            var englishExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id != id && c.NameEn == trimmedNameEn, cancellationToken);

            if (englishExists)
            {
                throw new CategoryDuplicateNameException(trimmedNameEn, "English");
            }
        }

        category.Update(
            nameAr: trimmedNameAr,
            nameEn: trimmedNameEn,
            descriptionAr: request.DescriptionAr,
            descriptionEn: request.DescriptionEn,
            nowUtc: DateTime.UtcNow);

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    /// <summary>
    /// Deletes a category ensuring it is not associated with any campaigns (CAT-01 / CAT-04).
    /// </summary>
    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        // Domain Invariant 10: Check if category has associated campaigns (including soft-deleted)
        var hasCampaigns = await _context.Campaigns
            .IgnoreQueryFilters()
            .AnyAsync(c => c.CategoryId == id, cancellationToken);

        try
        {
            category.EnsureCanBeDeleted(hasCampaigns);
        }
        catch (DomainException)
        {
            // Translate the domain rule into the API contract required by CAT-04 (HTTP 409 Conflict)
            throw new CategoryHasCampaignsException(id);
        }

        _categoryRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CategoryDto MapToDto(Category category) => new()
    {
        Id = category.Id,
        NameAr = category.NameAr,
        NameEn = category.NameEn,
        DescriptionAr = category.DescriptionAr,
        DescriptionEn = category.DescriptionEn,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt
    };
}

