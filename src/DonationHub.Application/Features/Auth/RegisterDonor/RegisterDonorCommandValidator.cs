using FluentValidation;

namespace DonationHub.Application.Features.Auth.RegisterDonor;

/// <summary>
/// Enforces registration constraints including password complexity and email format (IDN-02).
/// </summary>
public sealed class RegisterDonorCommandValidator : AbstractValidator<RegisterDonorCommand>
{
    public RegisterDonorCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.PreferredLanguage)
            .Must(lang => string.IsNullOrWhiteSpace(lang) || lang is "en" or "ar")
            .WithMessage("Preferred language must be either 'en' or 'ar'.");
    }
}
