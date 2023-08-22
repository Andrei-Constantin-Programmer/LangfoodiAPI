using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

public interface IRecipeRepository
{
    RecipeAggregate? GetRecipeById(string id);
    IEnumerable<RecipeAggregate> GetRecipesByChef(string chefId);
    RecipeAggregate CreateRecipe(string title, Recipe recipe, string shortDescription, string longDescription, User chef, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate, ISet<string> labels);
    bool UpdateRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(string id);
}
