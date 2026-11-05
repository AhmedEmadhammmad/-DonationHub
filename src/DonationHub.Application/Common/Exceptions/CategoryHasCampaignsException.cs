namespace DonationHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when attempting to delete a category that has associated campaigns (CAT-04 / Invariant 10).
/// </summary>
public class CategoryHasCampaignsException : Exception
{
    public const string ErrorCode = "Category.HasCampaigns";

    /// <summary>
    /// Initializes a new exception instance indicating category cannot be deleted due to associated campaigns.
    /// </summary>
    public CategoryHasCampaignsException(Guid categoryId)
        : base($"Cannot delete category with ID '{categoryId}' because it has active or historic campaigns associated with it.")
    {
    }
}

