using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipePersistenceRepository
{
    RecipeAggregate CreateRecipe(string title, Recipe recipe, string description, User chef, ISet<string> labels, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate);
    bool UpdateRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(RecipeAggregate recipe);
    bool DeleteRecipe(string id);
}
