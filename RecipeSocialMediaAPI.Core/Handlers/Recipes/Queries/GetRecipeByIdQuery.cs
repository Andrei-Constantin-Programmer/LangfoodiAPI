using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Exceptions;

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
