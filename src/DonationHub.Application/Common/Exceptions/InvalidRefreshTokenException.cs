namespace DonationHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a refresh token is expired, revoked, or non-existent (IDN-06).
/// </summary>
public class InvalidRefreshTokenException : Exception
{
    public const string ErrorCode = "Auth.RefreshTokenInvalid";

    /// <summary>
    /// Initializes a new exception instance indicating the refresh token cannot be accepted.
    /// </summary>
    public InvalidRefreshTokenException()
        : base("The provided refresh token is invalid, expired, or has already been revoked.")
    {
    }
}

