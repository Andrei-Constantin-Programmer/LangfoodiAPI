using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Images;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Commands;

public record class RemoveImagesCommand(List<string> PublicIds) : IRequest;

internal class RemoveImagesHandler : IRequestHandler<RemoveImagesCommand>
{
    private readonly IImageHostingPersistenceRepository _imageHostingPersistenceRepository;

    public RemoveImagesHandler(IImageHostingPersistenceRepository imageHostingPersistenceRepository)
    {
        _imageHostingPersistenceRepository = imageHostingPersistenceRepository;
    }

    public Task Handle(RemoveImagesCommand request, CancellationToken cancellationToken)
    {
        bool isSuccessful = request.PublicIds.Count() > 0
            && _imageHostingPersistenceRepository.BulkRemoveHostedImages(request.PublicIds);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception("Could not remove images or only partially removed some images");
    }
}