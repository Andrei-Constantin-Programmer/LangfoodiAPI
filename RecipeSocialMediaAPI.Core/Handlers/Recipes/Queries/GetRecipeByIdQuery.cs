using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.DTO.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipeByIdQuery(int Id) : IRequest<RecipeDetailedDTO>;

internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailedDTO>
{
    public GetRecipeByIdHandler(IRecipeRepository recipeRepository)
    {
    }

    public async Task<RecipeDetailedDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
