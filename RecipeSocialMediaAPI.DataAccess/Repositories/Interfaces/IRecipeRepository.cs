using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

public interface IRecipeRepository
{
    RecipeAggregate? GetRecipeById(string id);
    IEnumerable<RecipeAggregate> GetRecipesByChef(User? user);
    IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName);
    IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId);
    RecipeAggregate CreateRecipe(string title, Recipe recipe, string description, User chef, ISet<string> labels, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate);
    bool UpdateRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(string id);
}
