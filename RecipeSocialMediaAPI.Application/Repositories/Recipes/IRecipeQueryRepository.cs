using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipeQueryRepository
{
    RecipeAggregate? GetRecipeById(string id);
    IEnumerable<RecipeAggregate> GetRecipesByChef(User? user);
    IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName);
    IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId);
}
