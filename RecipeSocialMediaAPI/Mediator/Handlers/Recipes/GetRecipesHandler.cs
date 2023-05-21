using MediatR;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Mediator.Queries.Recipes;

namespace RecipeSocialMediaAPI.Mediator.Handlers.Recipes
{
    public class GetRecipesHandler : IRequestHandler<GetRecipesQuery, IEnumerable<RecipeDTO>>
    {
        private readonly IRecipeRepository _fakeRepository;

        public GetRecipesHandler(IRecipeRepository fakeRepository)
        {
            _fakeRepository = fakeRepository;
        }

        public async Task<IEnumerable<RecipeDTO>> Handle(GetRecipesQuery request, CancellationToken cancellationToken)
        {
            return (await _fakeRepository
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
