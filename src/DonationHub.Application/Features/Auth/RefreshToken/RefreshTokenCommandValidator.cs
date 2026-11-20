using FluentValidation;

namespace DonationHub.Application.Features.Auth.RefreshToken;

/// <summary>
/// Validator ensuring refresh token payload is present.
/// </summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
