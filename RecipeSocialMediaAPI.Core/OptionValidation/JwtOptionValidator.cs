using FluentValidation;
using RecipeSocialMediaAPI.Core.Options;

namespace RecipeSocialMediaAPI.Core.OptionValidation;

public sealed class JwtOptionValidator : AbstractValidator<JwtOptions>
{
    public JwtOptionValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("The JWT key cannot be empty");

        RuleFor(x => x.Issuer)
            .NotEmpty()
            .WithMessage("The JWT issuer cannot be empty");

        RuleFor(x => x.Audience)
            .NotEmpty()
            .WithMessage("The JWT audience cannot be empty");
    }
}
