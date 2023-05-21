using RecipeSocialMediaAPI.Data;

namespace RecipeSocialMediaAPI.DAL.Repositories
{
    public interface IRecipeRepository
    {
        Task CreateRecipe(Recipe recipe);
        Task<IEnumerable<Recipe>> GetAllRecipes();
    }
}