using FluentValidation;
using RecipeSocialMediaAPI.Application.Options;

namespace RecipeSocialMediaAPI.Core.OptionValidation;

public sealed class CloudinaryEndpointOptionValidator : AbstractValidator<CloudinaryEndpointOptions>
{
    public CloudinaryEndpointOptionValidator()
    {
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
