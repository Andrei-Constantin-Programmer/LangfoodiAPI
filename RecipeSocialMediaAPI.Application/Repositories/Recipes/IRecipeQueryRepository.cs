using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipeQueryRepository
{
    Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Recipe>> GetRecipesByChefAsync(IUserAccount? chef, CancellationToken cancellationToken = default);
    Task<IEnumerable<Recipe>> GetRecipesByChefIdAsync(string chefId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Recipe>> GetRecipesByChefNameAsync(string chefName, CancellationToken cancellationToken = default);
}
