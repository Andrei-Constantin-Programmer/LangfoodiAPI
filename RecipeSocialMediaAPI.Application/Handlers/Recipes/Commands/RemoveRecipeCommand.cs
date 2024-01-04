using MediatR;
using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Images;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;

public record RemoveRecipeCommand(string Id) : IRequest;

internal class RemoveRecipeHandler : IRequestHandler<RemoveRecipeCommand>
{
    private readonly ILogger<RemoveRecipeCommand> _logger;
    private readonly IRecipePersistenceRepository _recipePersistenceRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly IImageHostingPersistenceRepository _imageHostingPersistenceRepository;

    public RemoveRecipeHandler(IRecipePersistenceRepository recipePersistenceRepository, IRecipeQueryRepository recipeQueryRepository, IImageHostingPersistenceRepository imageHostingPersistenceRepository, ILogger<RemoveRecipeCommand> logger)
    {
        _recipePersistenceRepository = recipePersistenceRepository;
        _recipeQueryRepository = recipeQueryRepository;
        _imageHostingPersistenceRepository = imageHostingPersistenceRepository;
        _logger = logger;
    }

    public Task Handle(RemoveRecipeCommand request, CancellationToken cancellationToken)
    {
        RecipeAggregate? recipeToRemove = _recipeQueryRepository.GetRecipeById(request.Id);
        if (recipeToRemove is null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        List<string> imageIds = recipeToRemove.Recipe.Steps
            .Where(x => x.Image is not null)
            .Select(r => r.Image!.ImageUrl)
            .ToList();

        if (recipeToRemove.ThumbnailId is not null)
        {
            imageIds.Add(recipeToRemove.ThumbnailId);
        }

        bool isRecipeRemoved = _recipePersistenceRepository.DeleteRecipe(request.Id);
        bool areImagesRemoved = _imageHostingPersistenceRepository.BulkRemoveHostedImages(imageIds);

        if (!areImagesRemoved)
        {
            _logger.LogWarning("Some of the recipe's related images failed to be removed, ids: {imageIds}", string.Join(",",imageIds));
        }

        return isRecipeRemoved
            ? Task.CompletedTask
            : throw new Exception($"Could not remove recipe with id {request.Id}.");
    }
}
