using FluentValidation;
using RecipeSocialMediaAPI.Application.Options;

namespace RecipeSocialMediaAPI.Core.OptionValidation;

public sealed class CloudinaryOptionValidator : AbstractValidator<CloudinaryOptions>
{
    public CloudinaryOptionValidator()
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

        RuleFor(x => x.SingleRemoveUrl)
            .NotEmpty()
            .WithMessage("The URL for single image removal cannot be empty.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("The URL for single image removal must be correctly formatted.");

        RuleFor(x => x.BulkRemoveUrl)
            .NotEmpty()
            .WithMessage("The URL for bulk image removal cannot be empty.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("The URL for bulk image removal must be correctly formatted.");
    }
}
 