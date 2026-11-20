namespace DonationHub.Domain.Common.Results;

/// <summary>
/// Categorization of system errors mapped to standard HTTP status codes.
/// </summary>
public enum ErrorType
{
    Validation = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    PayloadTooLarge = 413,
    UnprocessableEntity = 422,
    TooManyRequests = 429,
    Failure = 500
}
