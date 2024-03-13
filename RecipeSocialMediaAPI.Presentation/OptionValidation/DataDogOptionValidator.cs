using FluentValidation;
using RecipeSocialMediaAPI.Presentation.Options;

namespace RecipeSocialMediaAPI.Presentation.OptionValidation;

public sealed class DataDogOptionValidator : AbstractValidator<DataDogOptions>
{
    private const int DATADOG_API_KEY_LENGTH = 32;

    public DataDogOptionValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage("The DataDog API key cannot be empty.")
            .Length(DATADOG_API_KEY_LENGTH)
            .WithMessage($"The DataDog API key must be of size {DATADOG_API_KEY_LENGTH}.")
            .Must(key => !string.IsNullOrEmpty(key)
                && key.All(character => 
                    char.IsLower(character) 
                    || char.IsDigit(character)))
            .WithMessage("The DataDog API key must be alphanumeric (lowercase).");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("The DataDog URL cannot be empty.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("The DataDog URL must be correctly formatted.");

        RuleFor(x => x.Service)
            .NotEmpty()
            .WithMessage("The DataDog service name cannot be empty.")
            .Must(service => !string.IsNullOrEmpty(service)
                && service.All(char.IsLetter))
            .WithMessage("The DataDog service name must be alphabetic.");
    }
}
