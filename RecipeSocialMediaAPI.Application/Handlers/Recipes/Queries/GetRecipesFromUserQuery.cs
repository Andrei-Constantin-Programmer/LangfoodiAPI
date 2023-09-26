using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

public record GetRecipesFromUserQuery(string Username) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserHandler : IRequestHandler<GetRecipesFromUserQuery, IEnumerable<RecipeDTO>>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public GetRecipesFromUserHandler(IRecipeMapper mapper, IRecipeQueryRepository recipeRepository)
    {
        _recipeQueryRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(
            _recipeQueryRepository.GetRecipesByChefName(request.Username)
            .Select(_mapper.MapRecipeAggregateToRecipeDto));
    }
}
