using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Commands;

public record class RemoveImagesCommand(List<string> PublicIds) : IRequest;

internal class RemoveImagesHandler : IRequestHandler<RemoveImagesCommand>
{
    private readonly IImageHostingQueryRepository _imageHostingQueryRepository;

    public RemoveImagesHandler(IImageHostingQueryRepository imageHostingQueryRepository)
    {
        _imageHostingQueryRepository = imageHostingQueryRepository;
    }

    public Task Handle(RemoveImagesCommand request, CancellationToken cancellationToken)
    {
        bool isSuccessful = _imageHostingQueryRepository.BulkRemoveHostedImages(request.PublicIds);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception("Could not remove images or only partially removed some images");
    }
}