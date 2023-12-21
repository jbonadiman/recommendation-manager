using RecommendationManager.Application.Models.Books;

namespace RecommendationManager.Application.Validator;

public class SaveBundleRequestValidator : Validator<SaveBundleRequest>
{
    public SaveBundleRequestValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .SetValidator(new SaveRequestValidator());
    }
}
