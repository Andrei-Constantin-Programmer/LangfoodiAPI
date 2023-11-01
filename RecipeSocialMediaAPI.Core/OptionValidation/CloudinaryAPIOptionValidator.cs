using FluentValidation;
using RecipeSocialMediaAPI.DataAccess.Helpers;

namespace RecipeSocialMediaAPI.Core.OptionValidation;

public sealed class CloudinaryApiOptionValidator : AbstractValidator<CloudinaryApiOptions>
{
    public CloudinaryApiOptionValidator()
    {
        RuleFor(x => x.CloudName)
            .NotEmpty();

        RuleFor(x => x.ApiKey)
            .NotEmpty();

        RuleFor(x => x.ApiSecret)
            .NotEmpty();
    }
}
