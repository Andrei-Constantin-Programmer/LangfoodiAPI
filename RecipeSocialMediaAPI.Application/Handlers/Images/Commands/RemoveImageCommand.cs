using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Images;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Commands;

public record class RemoveImageCommand(string PublicId) : IRequest;

internal class RemoveImageHandler : IRequestHandler<RemoveImageCommand>
{
    private readonly IImageHostingPersistenceRepository _imageHostingPersistenceRepository;

    public RemoveImageHandler(IImageHostingPersistenceRepository imageHostingPersistenceRepository)
    {
        _imageHostingPersistenceRepository = imageHostingPersistenceRepository;
    }

    public Task Handle(RemoveImageCommand request, CancellationToken cancellationToken)
    {
        bool isSuccessful = _imageHostingPersistenceRepository.RemoveHostedImage(request.PublicId);

        return isSuccessful
            ? Task.CompletedTask
            : throw new ImageRemovalException(request.PublicId);
    }
}