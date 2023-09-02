using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using AutoMapper;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

public record GetRecipesFromUserQuery(string Username) : IRequest<IEnumerable<RecipeDTO>>;

internal class GetRecipesFromUserHandler : IRequestHandler<GetRecipesFromUserQuery, IEnumerable<RecipeDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesFromUserHandler(IMapper mapper, IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesFromUserQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<RecipeAggregate> recipes = _recipeRepository.GetRecipesByChefName(request.Username);
        return await Task.FromResult(_mapper.Map<IEnumerable<RecipeDTO>>(recipes));
    }
}
