using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipeQueryRepository
{
    RecipeAggregate? GetRecipeById(string id);
    IEnumerable<RecipeAggregate> GetRecipesByChef(IUserAccount? chef);
    IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId);
    IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName);
}
