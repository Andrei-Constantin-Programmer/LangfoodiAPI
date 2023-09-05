using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Mappers.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

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
        IEnumerable<RecipeAggregate> recipes = _recipeRepository.GetRecipesByChefId(request.Id);
        return await Task.FromResult(recipes.Select(_mapper.RecipeAggregateToRecipeDtoMapper.MapRecipeAggregateToRecipeDto));
    }
}