using FluentValidation;

namespace DonationHub.Application.Features.Auth.Logout;

/// <summary>
/// Validator ensuring the refresh token to revoke is provided.
/// </summary>
public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
