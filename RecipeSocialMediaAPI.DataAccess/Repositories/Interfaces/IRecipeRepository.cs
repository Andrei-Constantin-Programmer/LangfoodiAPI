using RecipeSocialMediaAPI.Domain.Entities;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

public interface IRecipeRepository
{
    Task CreateRecipe(Recipe recipe);
    Task<IEnumerable<Recipe>> GetAllRecipes();
    Task<Recipe?> GetRecipeById(int id);
}