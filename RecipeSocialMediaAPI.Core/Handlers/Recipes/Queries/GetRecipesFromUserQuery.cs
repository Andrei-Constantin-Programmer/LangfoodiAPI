using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipesFromUserQuery(string Username) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserHandler : IRequestHandler<GetRecipesFromUserQuery, IEnumerable<RecipeDTO>>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesFromUserHandler(IRecipeMapper mapper, IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(
            _recipeRepository.GetRecipesByChefName(request.Username)
            .Select(_mapper.MapRecipeAggregateToRecipeDto));
    }
}
