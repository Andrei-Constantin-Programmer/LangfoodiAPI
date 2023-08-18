using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.DTO.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipesFromUserQuery(string Username) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserHandler : IRequestHandler<GetRecipesFromUserQuery, IEnumerable<RecipeDTO>>
{
    public GetRecipesFromUserHandler(IRecipeRepository recipeRepository)
    {
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
