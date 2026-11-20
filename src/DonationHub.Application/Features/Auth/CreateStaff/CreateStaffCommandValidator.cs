using DonationHub.Domain.Entities;
using FluentValidation;

namespace DonationHub.Application.Features.Auth.CreateStaff;

/// <summary>
/// Validates staff member creation constraints and role restrictions (IDN-08).
/// </summary>
public sealed class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Temporary password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Staff role is required.")
            .Must(r => r is ApplicationUser.RoleAdmin or ApplicationUser.RoleCampaignManager)
            .WithMessage($"Role must be either '{ApplicationUser.RoleAdmin}' or '{ApplicationUser.RoleCampaignManager}'.");

        RuleFor(x => x.PreferredLanguage)
            .Must(lang => string.IsNullOrWhiteSpace(lang) || lang is "en" or "ar")
            .WithMessage("Preferred language must be either 'en' or 'ar'.");
    }
}
