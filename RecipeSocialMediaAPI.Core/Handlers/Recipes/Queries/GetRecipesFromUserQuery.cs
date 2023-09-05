using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using AutoMapper;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipesFromUserQuery(string Username) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserHandler : IRequestHandler<GetRecipesFromUserQuery, IEnumerable<RecipeDTO>>
{
    private readonly IRecipeAggregateToRecipeDtoMapper _aggregateMapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesFromUserHandler(IRecipeAggregateToRecipeDtoMapper aggregateMapper, IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
        _aggregateMapper = aggregateMapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<RecipeAggregate> recipes = _recipeRepository.GetRecipesByChefName(request.Username);
        return await Task.FromResult(recipes.Select(_aggregateMapper.MapRecipeAggregateToRecipeDto));
    }
}
