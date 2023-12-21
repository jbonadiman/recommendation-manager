using FluentValidation;
using RecommendationManager.Application.Models.Books;

namespace RecommendationManager.Application.Validators;

public class SaveBundleRequestValidator : AbstractValidator<SaveBundleRequest>
{
    public SaveBundleRequestValidator()
    {
        RuleFor(req => req.Name)
            .NotEmpty();

        RuleForEach(req => req.Items)
            .NotEmpty();
    }
}
