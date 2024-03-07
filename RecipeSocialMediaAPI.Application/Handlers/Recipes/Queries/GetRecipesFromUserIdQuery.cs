using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

public record GetRecipesFromUserIdQuery(string Id) : IRequest<List<RecipeDTO>>;

internal class GetRecipesFromUserIdHandler : IRequestHandler<GetRecipesFromUserIdQuery, List<RecipeDTO>>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public GetRecipesFromUserIdHandler(IRecipeMapper mapper, IRecipeQueryRepository recipeQueryRepository)
    {
        _recipeQueryRepository = recipeQueryRepository;
        _mapper = mapper;
    }

    public async Task<List<RecipeDTO>> Handle(GetRecipesFromUserIdQuery request, CancellationToken cancellationToken)
    {
        return (await _recipeQueryRepository.GetRecipesByChefIdAsync(request.Id, cancellationToken))
            .Select(_mapper.MapRecipeAggregateToRecipeDto)
            .ToList();
    }
}
