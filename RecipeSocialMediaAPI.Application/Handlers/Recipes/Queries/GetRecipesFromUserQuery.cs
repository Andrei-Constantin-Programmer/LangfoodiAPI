using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

public record GetRecipesFromUserQuery(string Username) : IRequest<List<RecipeDTO>>;

internal class GetRecipesFromUserHandler : IRequestHandler<GetRecipesFromUserQuery, List<RecipeDTO>>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public GetRecipesFromUserHandler(IRecipeMapper mapper, IRecipeQueryRepository recipeQueryRepository)
    {
        _recipeQueryRepository = recipeQueryRepository;
        _mapper = mapper;
    }

    public async Task<List<RecipeDTO>> Handle(GetRecipesFromUserQuery request, CancellationToken cancellationToken)
    {
        return (await _recipeQueryRepository.GetRecipesByChefNameAsync(request.Username, cancellationToken))
            .Select(_mapper.MapRecipeAggregateToRecipeDto)
            .ToList();
    }
}
