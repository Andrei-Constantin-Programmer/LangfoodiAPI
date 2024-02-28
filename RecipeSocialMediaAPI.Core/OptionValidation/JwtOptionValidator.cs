using FluentValidation;
using RecipeSocialMediaAPI.Application.Options;
using System.Text;

namespace RecipeSocialMediaAPI.Core.OptionValidation;

public sealed class JwtOptionValidator : AbstractValidator<JwtOptions>
{
    public JwtOptionValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("The JWT key cannot be empty")
            .Must(key =>
            {
                var bytes = Encoding.ASCII.GetBytes(key);
                return bytes.Length * 8 >= 128;
            })
            .WithMessage("The JWT key must have at least 128 bits");

        RuleFor(x => x.Issuer)
            .NotEmpty()
            .WithMessage("The JWT issuer cannot be empty");

        RuleFor(x => x.Audience)
            .NotEmpty()
            .WithMessage("The JWT audience cannot be empty");
    }
}
