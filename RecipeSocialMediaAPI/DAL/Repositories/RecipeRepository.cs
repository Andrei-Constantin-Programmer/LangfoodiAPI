using RecipeSocialMediaAPI.Data;
using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.DAL.Repositories
{
    // TODO This method is a stub. To be replaced with a real repository once the Recipe model has been set.
    public class RecipeRepository : IRecipeRepository
    {
        private readonly List<Recipe> _recipes;

        private readonly IClock _dateTimeProvider;
        
        public RecipeRepository(IClock dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

            _recipes = new List<Recipe>()
            {
                new Recipe(10, "TestTitle1", "TestDesc1", "TestChef1", _dateTimeProvider.Now),
                new Recipe(20, "TestTitle2", "TestDesc2", "TestChef2", _dateTimeProvider.Now),
                new Recipe(30, "TestTitle3", "TestDesc3", "TestChef3", _dateTimeProvider.Now),
            };
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes() => await Task.FromResult(_recipes);

        public async Task<Recipe?> GetRecipeById(int id) => await Task.FromResult(_recipes.SingleOrDefault(recipe => recipe.Id == id));

        public async Task CreateRecipe(Recipe recipe)
        {
            _recipes.Add(recipe);

            await Task.CompletedTask;
        }
    }
}
