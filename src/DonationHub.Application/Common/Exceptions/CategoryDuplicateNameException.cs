namespace DonationHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a category name is already registered in Arabic or English (CAT-02).
/// </summary>
public class CategoryDuplicateNameException : Exception
{
    public const string ErrorCode = "Category.DuplicateName";

    /// <summary>
    /// Initializes a new exception instance indicating name collision.
    /// </summary>
    public CategoryDuplicateNameException(string name, string language)
        : base($"A category with {language} name '{name}' already exists.")
    {
    }
}

