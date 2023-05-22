using MediatR;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Handlers.Recipes.Querries
{
    public record GetRecipesQuery() : IRequest<IEnumerable<RecipeDTO>>;

    public class GetRecipesHandler : IRequestHandler<GetRecipesQuery, IEnumerable<RecipeDTO>>
    {
        private readonly IRecipeRepository _recipeRepository;

        public GetRecipesHandler(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesQuery request, CancellationToken cancellationToken)
        {
            return (await _recipeRepository
                .GetAllRecipes())
                .Select(recipe => new RecipeDTO()
                {
                    Id = recipe.Id,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Chef = recipe.Chef,
                    CreationDate = recipe.CreationDate
                });
        }
    }
}
