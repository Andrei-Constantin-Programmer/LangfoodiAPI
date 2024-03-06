using FluentValidation;
using RecipeSocialMediaAPI.Infrastructure.Helpers;

namespace RecipeSocialMediaAPI.Presentation.OptionValidation;

public sealed class CloudinaryApiOptionValidator : AbstractValidator<CloudinaryApiOptions>
{
    public CloudinaryApiOptionValidator()
    {
        RuleFor(x => x.CloudName)
            .NotEmpty()
            .WithMessage("The CloudName cannot be empty.");

        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage("The ApiKey cannot be empty.");

        RuleFor(x => x.ApiSecret)
            .NotEmpty()
            .WithMessage("The ApiSecret cannot be empty.");
    }
}
 