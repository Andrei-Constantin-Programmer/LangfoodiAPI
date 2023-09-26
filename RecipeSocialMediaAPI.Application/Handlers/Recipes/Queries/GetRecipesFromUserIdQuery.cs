using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

public record GetRecipesFromUserIdQuery(string Id) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserIdHandler : IRequestHandler<GetRecipesFromUserIdQuery, IEnumerable<RecipeDTO>>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public GetRecipesFromUserIdHandler(IRecipeMapper mapper, IRecipeQueryRepository recipeRepository)
    {
        _recipeQueryRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserIdQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(
            _recipeQueryRepository.GetRecipesByChefId(request.Id)
            .Select(_mapper.MapRecipeAggregateToRecipeDto));
    }
}