using RecipeSocialMediaAPI.Data;

namespace RecipeSocialMediaAPI.DAL.Repositories
{
    public interface IRecipeRepository
    {
        Task AddRecipe(Recipe recipe);
        Task<IEnumerable<Recipe>> GetAllRecipes();
    }
}