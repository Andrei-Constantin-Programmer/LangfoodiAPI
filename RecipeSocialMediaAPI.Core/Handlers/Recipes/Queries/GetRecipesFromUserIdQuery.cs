using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipesFromUserIdQuery(string Id) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserIdHandler : IRequestHandler<GetRecipesFromUserIdQuery, IEnumerable<RecipeDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesFromUserIdHandler(IMapper mapper, IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<RecipeAggregate> recipes = _recipeRepository.GetRecipesByChefId(request.Id);
        return await Task.FromResult(_mapper.Map<IEnumerable<RecipeDTO>>(recipes));
    }
}