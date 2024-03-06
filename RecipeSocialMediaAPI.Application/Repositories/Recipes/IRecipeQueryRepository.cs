using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipeQueryRepository
{
    Task<RecipeAggregate?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeAggregate>> GetRecipesByChefAsync(IUserAccount? chef, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeAggregate>> GetRecipesByChefIdAsync(string chefId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeAggregate>> GetRecipesByChefNameAsync(string chefName, CancellationToken cancellationToken = default);
}
