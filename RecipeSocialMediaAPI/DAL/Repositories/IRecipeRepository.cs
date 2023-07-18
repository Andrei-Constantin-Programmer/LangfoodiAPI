using RecipeSocialMediaAPI.DataModels;

namespace RecipeSocialMediaAPI.DAL.Repositories;

internal interface IRecipeRepository
{
    Task CreateRecipe(Recipe recipe);
    Task<IEnumerable<Recipe>> GetAllRecipes();
    Task<Recipe?> GetRecipeById(int id);
}