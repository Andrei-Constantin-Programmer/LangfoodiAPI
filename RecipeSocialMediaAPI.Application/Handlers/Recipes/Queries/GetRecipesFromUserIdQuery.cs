using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

public record GetRecipesFromUserIdQuery(string Id) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserIdHandler : IRequestHandler<GetRecipesFromUserIdQuery, IEnumerable<RecipeDTO>>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesFromUserIdHandler(IRecipeMapper mapper, IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserIdQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(
            _recipeRepository.GetRecipesByChefId(request.Id)
            .Select(_mapper.MapRecipeAggregateToRecipeDto));
    }
}