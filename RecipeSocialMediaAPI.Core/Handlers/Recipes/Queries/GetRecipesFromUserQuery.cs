using MediatR;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

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
        IEnumerable<RecipeAggregate> recipes = _recipeRepository.GetRecipesByChefName(request.Username);
        return await Task.FromResult(recipes.Select(_mapper.RecipeAggregateToRecipeDtoMapper.MapRecipeAggregateToRecipeDto));
    }
}
