using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.CreateCampaign;

/// <summary>
/// Validator ensuring structural completeness and positivity of campaign goals (CMP-01).
/// </summary>
public sealed class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
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

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty().WithMessage("CreatedByUserId is required.");
    }
}
