using DonationHub.Domain.Common.Results;

namespace DonationHub.Domain.Common.Errors;

public static class ValidationErrors
{
    public static readonly Error Failed =
        new("Validation.Failed", "One or more validation errors occurred.", ErrorType.Validation);
}

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized);

    public static readonly Error EmailAlreadyExists =
        new("Auth.EmailAlreadyExists", "A user with this email already exists.", ErrorType.Conflict);

    public static readonly Error RefreshTokenInvalid =
        new("Auth.RefreshTokenInvalid", "The refresh token is invalid or expired.", ErrorType.Unauthorized);

    public static readonly Error AccountDeactivated =
        new("Auth.AccountDeactivated", "This account has been deactivated.", ErrorType.Forbidden);
}

public static class CategoryErrors
{
    public static readonly Error NotFound =
        new("Category.NotFound", "Category was not found.", ErrorType.NotFound);

    public static readonly Error HasCampaigns =
        new("Category.HasCampaigns", "Cannot delete a category with linked campaigns.", ErrorType.Conflict);

    public static readonly Error DuplicateName =
        new("Category.DuplicateName", "A category with the specified Arabic or English name already exists.", ErrorType.Conflict);
}

public static class CampaignErrors
{
    public static readonly Error NotFound =
        new("Campaign.NotFound", "Campaign was not found.", ErrorType.NotFound);

    public static readonly Error NotActive =
        new("Campaign.NotActive", "Campaign is completed or expired.", ErrorType.Conflict);

    public static readonly Error GoalBelowRaised =
        new("Campaign.GoalBelowRaised", "Goal cannot be lower than the raised amount.", ErrorType.UnprocessableEntity);

    public static readonly Error EndDateInPast =
        new("Campaign.EndDateInPast", "End date must be in the future.", ErrorType.UnprocessableEntity);

    public static readonly Error ConcurrencyConflict =
        new("Campaign.ConcurrencyConflict", "Concurrent update detected. The campaign has been modified by another process.", ErrorType.Conflict);
}

public static class DonationErrors
{
    public static readonly Error NotFound =
        new("Donation.NotFound", "Donation was not found or not owned.", ErrorType.NotFound);

    public static readonly Error AmountOutOfRange =
        new("Donation.AmountOutOfRange", "Amount is outside the configured donation range.", ErrorType.UnprocessableEntity);

    public static readonly Error InvalidState =
        new("Donation.InvalidState", "Donation is already in a final state.", ErrorType.Conflict);
}

public static class CommonErrors
{
    public static readonly Error RateLimited =
        new("Common.RateLimited", "Rate limit exceeded.", ErrorType.TooManyRequests);
}

public static class FileErrors
{
    public static readonly Error InvalidType =
        new("File.InvalidType", "File signature not allowed.", ErrorType.Validation);

    public static readonly Error TooLarge =
        new("File.TooLarge", "File exceeds the maximum allowed size.", ErrorType.PayloadTooLarge);
}

public static class PaymentErrors
{
    public static readonly Error InvalidSignature =
        new("Payment.InvalidSignature", "Stripe webhook signature invalid.", ErrorType.Validation);
}

public static class IdempotencyErrors
{
    public static readonly Error KeyReusedWithDifferentRequest =
        new("Idempotency.KeyReusedWithDifferentRequest", "Same Idempotency-Key with a different body.", ErrorType.UnprocessableEntity);
}
