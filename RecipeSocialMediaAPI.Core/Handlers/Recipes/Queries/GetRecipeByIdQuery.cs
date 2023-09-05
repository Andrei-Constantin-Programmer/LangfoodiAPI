using MediatR;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipeByIdQuery(string Id) : IRequest<RecipeDetailedDTO>;

internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailedDTO>
{
    private readonly IRecipeAggregateToRecipeDetailedDtoMapper _aggregateMapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipeByIdHandler(IRecipeAggregateToRecipeDetailedDtoMapper aggregateMapper, IRecipeRepository recipeRepository)
    {
        _aggregateMapper = aggregateMapper;
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        RecipeAggregate? recipe = _recipeRepository.GetRecipeById(request.Id);

        if (recipe is null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        return await Task.FromResult(_aggregateMapper.MapRecipeAggregateToRecipeDetailedDto(recipe));
    }
}
