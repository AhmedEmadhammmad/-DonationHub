using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.UpdateCampaign;

/// <summary>
/// Validates update request boundaries and presence of RowVersion concurrency token (CMP-02, CMP-10).
/// </summary>
public sealed class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Campaign ID is required.");

        RuleFor(x => x.TitleAr)
            .NotEmpty().WithMessage("Arabic title is required.")
            .MaximumLength(200).WithMessage("Arabic title cannot exceed 200 characters.");

        RuleFor(x => x.TitleEn)
            .NotEmpty().WithMessage("English title is required.")
            .MaximumLength(200).WithMessage("English title cannot exceed 200 characters.");

        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Arabic description is required.");

        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("English description is required.");

        RuleFor(x => x.GoalAmount)
            .GreaterThan(0).WithMessage("Goal amount must be strictly greater than zero.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.RowVersion)
            .NotEmpty().WithMessage("RowVersion concurrency token is required.");
    }
}
