using FluentValidation;
using RecipeSocialMediaAPI.Application.Options;
using System.Text;

namespace RecipeSocialMediaAPI.Presentation.OptionValidation;

public sealed class EncryptionOptionValidator : AbstractValidator<EncryptionOptions>
{
    public EncryptionOptionValidator()
    {
        RuleFor(x => x.EncryptionKey)
            .NotEmpty()
            .WithMessage("Encryption key cannot be empty")
            .Must(key =>
            {
                var bytes = Encoding.UTF8.GetBytes(key);
                return bytes.Length * 8 >= 256;
            })
            .WithMessage("The encryption key must have exactly 256 bits");
    }
}
