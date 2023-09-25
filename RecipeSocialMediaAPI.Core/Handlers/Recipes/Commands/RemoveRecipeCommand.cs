using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record RemoveRecipeCommand(string Id) : IRequest;

internal class RemoveRecipeHandler : IRequestHandler<RemoveRecipeCommand>
{
    private readonly IRecipeRepository _recipeRepository;

    public RemoveRecipeHandler(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public Task Handle(RemoveRecipeCommand request, CancellationToken cancellationToken)
    {
        if (_recipeRepository.GetRecipeById(request.Id) is null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        bool isSuccessful = _recipeRepository.DeleteRecipe(request.Id);

        return isSuccessful
            ? Task.CompletedTask
            : throw new Exception($"Could not remove recipe with id {request.Id}.");
    }
}
