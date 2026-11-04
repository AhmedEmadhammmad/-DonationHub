namespace DonationHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a deactivated user attempts to authenticate or refresh tokens (IDN-10).
/// </summary>
public class AccountDeactivatedException : Exception
{
    public const string ErrorCode = "Auth.AccountDeactivated";

    /// <summary>
    /// Initializes a new exception instance indicating the user account is disabled.
    /// </summary>
    public AccountDeactivatedException()
        : base("Your account has been deactivated. Please contact support.")
    {
    }
}

