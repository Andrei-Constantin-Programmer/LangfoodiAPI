using MediatR;
using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;

public record RemoveRecipeCommand(string Id) : IRequest;

internal class RemoveRecipeHandler : IRequestHandler<RemoveRecipeCommand>
{
    private readonly ILogger<RemoveRecipeCommand> _logger;
    private readonly IRecipePersistenceRepository _recipePersistenceRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;
    private readonly ICloudinaryWebClient _cloudinaryWebClient;
    private readonly IPublisher _publisher;

    public RemoveRecipeHandler(
        IRecipePersistenceRepository recipePersistenceRepository,
        IRecipeQueryRepository recipeQueryRepository,
        ICloudinaryWebClient cloudinaryWebClient,
        ILogger<RemoveRecipeCommand> logger,
        IPublisher publisher)
    {
        _recipePersistenceRepository = recipePersistenceRepository;
        _recipeQueryRepository = recipeQueryRepository;
        _cloudinaryWebClient = cloudinaryWebClient;
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(RemoveRecipeCommand request, CancellationToken cancellationToken)
    {
        Recipe? recipeToRemove = await _recipeQueryRepository.GetRecipeByIdAsync(request.Id, cancellationToken) 
            ?? throw new RecipeNotFoundException(request.Id);

        var imageIds = recipeToRemove.Guide.Steps
            .Where(x => x.Image is not null)
            .Select(r => r.Image!.ImageUrl)
            .ToList();

        if (recipeToRemove.ThumbnailId is not null)
        {
            imageIds.Add(recipeToRemove.ThumbnailId);
        }

        await _publisher.Publish(new RecipeRemovedNotification(recipeToRemove.Id), cancellationToken);

        bool isRecipeRemoved = await _recipePersistenceRepository.DeleteRecipeAsync(request.Id, cancellationToken);
        bool areImagesRemoved = imageIds.Count <= 0 || _cloudinaryWebClient.BulkRemoveHostedImages(imageIds);

        if (!areImagesRemoved)
        {
            _logger.LogWarning("Some of the images for recipe {RecipeId} failed to be removed, ids: {ImageIds}", recipeToRemove.Id, string.Join(",",imageIds));
        }

        if (!isRecipeRemoved)
        {
            throw new RecipeRemovalException(recipeToRemove.Id);
        }
    }
}
