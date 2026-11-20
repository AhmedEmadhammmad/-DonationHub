using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using FluentValidation;
using MediatR;

namespace DonationHub.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior executing registered FluentValidation validators before request dispatch.
/// Short-circuits with Validation.Failed and field-level errors when input rules fail.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var errorsDictionary = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
            );

        var validationError = Error.Validation(
            ValidationErrors.Failed.Code,
            ValidationErrors.Failed.Message,
            errorsDictionary);

        return CreateValidationResult(validationError);
    }

    private static TResponse CreateValidationResult(Error error)
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result)
                .GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
                .MakeGenericMethod(valueType);

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Type {responseType.FullName} is not a compatible Result type.");
    }
}
