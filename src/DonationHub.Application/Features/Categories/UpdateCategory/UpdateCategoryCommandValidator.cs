using FluentValidation;

namespace DonationHub.Application.Features.Categories.UpdateCategory;

/// <summary>
/// Validator verifying category update boundaries (CAT-02).
/// </summary>
public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic category name is required.")
            .MaximumLength(100).WithMessage("Arabic category name cannot exceed 100 characters.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English category name is required.")
            .MaximumLength(100).WithMessage("English category name cannot exceed 100 characters.");

        RuleFor(x => x.DescriptionAr)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.DescriptionAr))
            .WithMessage("Arabic description cannot exceed 500 characters.");

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.DescriptionEn))
            .WithMessage("English description cannot exceed 500 characters.");
    }
}
