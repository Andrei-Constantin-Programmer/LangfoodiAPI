using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Commands;

public record class RemoveImageCommand(string PublicId) : IRequest;

internal class RemoveImageHandler : IRequestHandler<RemoveImageCommand>
{
    private readonly ICloudinaryWebClient _cloudinaryWebClient;

    public RemoveImageHandler(ICloudinaryWebClient cloudinaryWebClient)
    {
        _cloudinaryWebClient = cloudinaryWebClient;
    }

    public Task Handle(RemoveImageCommand request, CancellationToken cancellationToken)
    {
        bool isSuccessful = _cloudinaryWebClient.RemoveHostedImage(request.PublicId);

        return isSuccessful
            ? Task.CompletedTask
            : throw new ImageRemovalException(request.PublicId);
    }
}
