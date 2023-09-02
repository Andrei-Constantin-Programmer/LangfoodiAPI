using MediatR;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;

public record RemoveRecipeCommand(string Id) : IRequest<bool>;

internal class RemoveRecipeHandler : IRequestHandler<RemoveRecipeCommand, bool>
{
    private readonly IRecipeRepository _recipeRepository;

    public RemoveRecipeHandler(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<bool> Handle(RemoveRecipeCommand request, CancellationToken cancellationToken)
    {
        if (_recipeRepository.GetRecipeById(request.Id) is null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        return await Task.FromResult(_recipeRepository.DeleteRecipe(request.Id));
    }
}
