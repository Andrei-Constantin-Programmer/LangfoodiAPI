using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

public record GetRecipeByIdQuery(string Id) : IRequest<RecipeDetailedDTO>;

internal class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailedDTO>
{
    private readonly IRecipeMapper _mapper;
    private readonly IRecipeQueryRepository _recipeQueryRepository;

    public GetRecipeByIdHandler(IRecipeMapper mapper, IRecipeQueryRepository recipeRepository)
    {
        _mapper = mapper;
        _recipeQueryRepository = recipeRepository;
    }

    public async Task<RecipeDetailedDTO> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        RecipeAggregate? recipe = _recipeQueryRepository.GetRecipeById(request.Id);

        return recipe is null
            ? throw new RecipeNotFoundException(request.Id)
            : await Task.FromResult(_mapper.MapRecipeAggregateToRecipeDetailedDto(recipe));
    }
}
