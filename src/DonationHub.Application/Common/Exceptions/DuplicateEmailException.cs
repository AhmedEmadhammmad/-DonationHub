namespace DonationHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when registration is attempted with an existing email address.
/// </summary>
public class DuplicateEmailException : Exception
{
    public const string ErrorCode = "Auth.EmailAlreadyExists";

    /// <summary>
    /// Initializes a new exception instance indicating the specified email is duplicate.
    /// </summary>
    public DuplicateEmailException(string email)
        : base($"An account with email '{email}' already exists.")
    {
    }
}

