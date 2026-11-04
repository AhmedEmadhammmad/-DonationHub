namespace DonationHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when authentication fails due to incorrect username or password (IDN-12).
/// </summary>
public class InvalidCredentialsException : Exception
{
    public const string ErrorCode = "Auth.InvalidCredentials";

    /// <summary>
    /// Initializes a new exception instance with standardized generic credentials message.
    /// </summary>
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    {
    }
}
