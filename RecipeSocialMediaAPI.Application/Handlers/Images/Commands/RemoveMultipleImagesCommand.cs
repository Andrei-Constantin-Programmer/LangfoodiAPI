using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Commands;

public record class RemoveMultipleImagesCommand(List<string> PublicIds) : IRequest;

internal class RemoveMultipleImagesHandler : IRequestHandler<RemoveMultipleImagesCommand>
{
    private readonly ICloudinaryWebClient _cloudinaryWebClient;

    public RemoveMultipleImagesHandler(ICloudinaryWebClient cloudinaryWebClient)
    {
        _cloudinaryWebClient = cloudinaryWebClient;
    }

    public Task Handle(RemoveMultipleImagesCommand request, CancellationToken cancellationToken)
    {
        bool isSuccessful = _cloudinaryWebClient.BulkRemoveHostedImages(request.PublicIds);

        return isSuccessful
            ? Task.CompletedTask
            : throw new MultipleImageRemovalException(request.PublicIds);
    }
}

public class RemoveMultipleImagesValidator : AbstractValidator<RemoveMultipleImagesCommand>
{
    public RemoveMultipleImagesValidator()
    {
        RuleFor(x => x.PublicIds)
            .NotEmpty();
    }
}