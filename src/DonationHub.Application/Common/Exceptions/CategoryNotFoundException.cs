namespace DonationHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested category identifier does not exist (CAT-04).
/// </summary>
public class CategoryNotFoundException : Exception
{
    public const string ErrorCode = "Category.NotFound";

    /// <summary>
    /// Initializes a new exception instance indicating missing category.
    /// </summary>
    public CategoryNotFoundException(Guid categoryId)
        : base($"Category with ID '{categoryId}' was not found.")
    {
    }
}

