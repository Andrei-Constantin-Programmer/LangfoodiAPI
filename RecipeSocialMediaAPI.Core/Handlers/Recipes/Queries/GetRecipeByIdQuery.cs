using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipeByIdQuery(string Id) : IRequest<RecipeDetailedDTO>;

internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailedDTO>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipeByIdHandler(IRecipeMapper mapper, IRecipeRepository recipeRepository)
    {
        _mapper = mapper;
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        RecipeAggregate? recipe = _recipeRepository.GetRecipeById(request.Id);

        if (recipe is null)
        {
            throw new RecipeNotFoundException(request.Id);
        }

        return await Task.FromResult(_mapper.MapRecipeAggregateToRecipeDetailedDto(recipe));
    }
}
