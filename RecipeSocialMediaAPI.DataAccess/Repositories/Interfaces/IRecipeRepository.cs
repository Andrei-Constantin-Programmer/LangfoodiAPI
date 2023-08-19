using RecipeSocialMediaAPI.Domain.Models.Recipes;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

public interface IRecipeRepository
{
    Recipe? GetRecipeById(string id);
    IEnumerable<Recipe> GetRecipesByChef(string chefId);
    Recipe CreateRecipe(Recipe recipe);
    bool UpdateRecipe(Recipe recipe);
    bool DeleteRecipe(Recipe recipe);
    bool DeleteRecipe(string id);
}
