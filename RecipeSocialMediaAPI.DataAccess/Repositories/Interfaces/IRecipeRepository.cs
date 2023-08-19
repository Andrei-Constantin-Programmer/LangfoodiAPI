using RecipeSocialMediaAPI.Domain.Models.Recipes;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

public interface IRecipeRepository
{
    RecipeAggregate? GetRecipeById(string id);
    IEnumerable<RecipeAggregate> GetRecipesByChef(string chefId);
    RecipeAggregate CreateRecipe(RecipeAggregate recipe);
    bool UpdateRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(string id);
}
