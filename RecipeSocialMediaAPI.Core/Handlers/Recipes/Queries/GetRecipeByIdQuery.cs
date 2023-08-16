using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.DTO.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

internal record GetRecipeByIdQuery(int Id) : IRequest<RecipeDTO>;

internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDTO>
{
    public GetRecipeByIdHandler(IRecipeRepository recipeRepository)
    {
    }

    public async Task<RecipeDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
