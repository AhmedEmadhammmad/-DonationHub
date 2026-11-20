namespace DonationHub.Domain.Common.Results;

/// <summary>
/// Immutable representation of a domain or application error.
/// </summary>
/// <param name="Code">Unique error code identifier from the error catalogue.</param>
/// <param name="Message">Descriptive, user-friendly or localized error message.</param>
/// <param name="Type">The semantic error category mapped to HTTP status codes.</param>
/// <param name="ValidationErrors">Optional field-level errors dictionary for validation failures.</param>
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Validation(string code, string message, IReadOnlyDictionary<string, string[]>? errors = null) =>
        new(code, message, ErrorType.Validation, errors);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    public static Error UnprocessableEntity(string code, string message) =>
        new(code, message, ErrorType.UnprocessableEntity);

    public static Error TooManyRequests(string code, string message) =>
        new(code, message, ErrorType.TooManyRequests);

    public static Error PayloadTooLarge(string code, string message) =>
        new(code, message, ErrorType.PayloadTooLarge);

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);
}
