using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipesFromUserIdQuery(string Id) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserIdHandler : IRequestHandler<GetRecipesFromUserIdQuery, IEnumerable<RecipeDTO>>
{
    private readonly IRecipeAggregateToRecipeDtoMapper _aggregateMapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesFromUserIdHandler(IRecipeAggregateToRecipeDtoMapper aggregateMapper, IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
        _aggregateMapper = aggregateMapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<RecipeAggregate> recipes = _recipeRepository.GetRecipesByChefId(request.Id);
        return await Task.FromResult(recipes.Select(_aggregateMapper.MapRecipeAggregateToRecipeDto));
    }
}