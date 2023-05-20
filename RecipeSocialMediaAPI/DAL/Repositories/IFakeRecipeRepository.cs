using RecipeSocialMediaAPI.Data;

namespace RecipeSocialMediaAPI.DAL.Repositories
{
    public interface IFakeRecipeRepository
    {
        Task AddRecipe(Recipe recipe);
        Task<IEnumerable<Recipe>> GetAllRecipes();
    }
}