using RecipeSocialMediaAPI.Data;
using RecipeSocialMediaAPI.Services.Interfaces;

namespace RecipeSocialMediaAPI.DAL.Repositories
{
    public class FakeRecipeRepository : IFakeRecipeRepository
    {
        private readonly List<Recipe> _recipes;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FakeRecipeRepository(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

            _recipes = new List<Recipe>()
            {
                new Recipe("TestTitle1", "TestDesc1", "TestChef1", _dateTimeProvider.Now),
                new Recipe("TestTitle2", "TestDesc2", "TestChef2", _dateTimeProvider.Now),
                new Recipe("TestTitle3", "TestDesc3", "TestChef3", _dateTimeProvider.Now),
            };
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes() => await Task.FromResult(_recipes);

        public async Task AddRecipe(Recipe recipe)
        {
            _recipes.Add(recipe);

            await Task.CompletedTask;
        }
    }
}
