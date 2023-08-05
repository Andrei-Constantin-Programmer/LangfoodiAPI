using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.DTO;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

internal record GetRecipesQuery() : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesHandler : IRequestHandler<GetRecipesQuery, IEnumerable<RecipeDTO>>
{
    public GetRecipesHandler(IRecipeRepository recipeRepository)
    {
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
