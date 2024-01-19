using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Images;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Commands;

public record class RemoveMultipleImagesCommand(List<string> PublicIds) : IRequest;

internal class RemoveMultipleImagesHandler : IRequestHandler<RemoveMultipleImagesCommand>
{
    private readonly IImageHostingPersistenceRepository _imageHostingPersistenceRepository;

    public RemoveMultipleImagesHandler(IImageHostingPersistenceRepository imageHostingPersistenceRepository)
    {
        _imageHostingPersistenceRepository = imageHostingPersistenceRepository;
    }

    public Task Handle(RemoveMultipleImagesCommand request, CancellationToken cancellationToken)
    {
        bool isSuccessful = _imageHostingPersistenceRepository.BulkRemoveHostedImages(request.PublicIds);

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