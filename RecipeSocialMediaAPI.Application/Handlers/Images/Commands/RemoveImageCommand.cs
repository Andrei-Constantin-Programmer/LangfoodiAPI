using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Commands;

public record class RemoveImageCommand(string PublicId) : IRequest;

internal class RemoveImageHandler : IRequestHandler<RemoveImageCommand>
{
    private readonly IImageHostingQueryRepository _imageHostingQueryRepository;

    public RemoveImageHandler(IImageHostingQueryRepository imageHostingQueryRepository)
    {
        _imageHostingQueryRepository = imageHostingQueryRepository;
    }

    public Task Handle(RemoveImageCommand request, CancellationToken cancellationToken)
    {
        bool isSuccessful = _imageHostingQueryRepository.RemoveHostedImage(request.PublicId);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception("Could not remove image");
    }
}