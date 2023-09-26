using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;

public record RemoveRecipeCommand(string Id) : IRequest;

internal class RemoveRecipeHandler : IRequestHandler<RemoveRecipeCommand>
{
    private readonly IRecipePersistenceRepository _recipePersistenceRepository;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public RemoveRecipeHandler(IRecipePersistenceRepository recipePersistenceRepository, IRecipeQueryRepository recipeQueryRepository)
    {
        _recipePersistenceRepository = recipePersistenceRepository;
        _recipeQueryRepository = recipeQueryRepository;
    }

    public Task Handle(RemoveRecipeCommand request, CancellationToken cancellationToken)
    {
        if (_recipeQueryRepository.GetRecipeById(request.Id) is null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        bool isSuccessful = _recipePersistenceRepository.DeleteRecipe(request.Id);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception($"Could not remove recipe with id {request.Id}.");
    }
}
