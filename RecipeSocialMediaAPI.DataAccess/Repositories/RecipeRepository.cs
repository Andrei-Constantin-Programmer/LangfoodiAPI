using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.DataAccess.Repositories;

// TODO This class is a stub. To be replaced with a real repository once the Recipe model has been set.
public class RecipeRepository : IRecipeRepository
{
    //private readonly DatabaseConfiguration _databaseConfiguration;

    //private readonly List<Recipe> _recipes;

    //public RecipeRepository(DatabaseConfiguration databaseConfiguration)
    //{
    //    _databaseConfiguration = databaseConfiguration;

    //    _recipes = new List<Recipe>()
    //    {
    //        new Recipe(10, "TestTitle1", "TestDesc1", "TestChef1", DateTimeOffset.UtcNow),
    //        new Recipe(20, "TestTitle2", "TestDesc2", "TestChef2", DateTimeOffset.UtcNow),
    //        new Recipe(30, "TestTitle3", "TestDesc3", "TestChef3", DateTimeOffset.UtcNow),
    //    };
    //}

    //public async Task<IEnumerable<Recipe>> GetAllRecipes() => await Task.FromResult(_recipes);

    //public async Task<Recipe?> GetRecipeById(int id) => await Task.FromResult(_recipes.SingleOrDefault(recipe => recipe.Id == id));

    //public async Task CreateRecipe(Recipe recipe)
    //{
    //    _recipes.Add(recipe);

    //    await Task.CompletedTask;
    //}
    public Task CreateRecipe(Recipe recipe) => throw new NotImplementedException();
    public Task<IEnumerable<Recipe>> GetAllRecipes() => throw new NotImplementedException();
    public Task<Recipe?> GetRecipeById(int id) => throw new NotImplementedException();
}
