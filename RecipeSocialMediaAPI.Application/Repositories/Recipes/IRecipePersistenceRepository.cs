using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Recipes;

public interface IRecipePersistenceRepository
{
    Task<RecipeAggregate> CreateRecipeAsync(
        string title,
        Recipe recipe,
        string description,
        IUserAccount chef,
        ISet<string> tags,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        string? thumbnailId,
        CancellationToken cancellationToken = default);
    Task<bool> UpdateRecipeAsync(RecipeAggregate recipe, CancellationToken cancellationToken = default);
    Task<bool> DeleteRecipeAsync(RecipeAggregate recipe, CancellationToken cancellationToken = default);
    Task<bool> DeleteRecipeAsync(string id, CancellationToken cancellationToken = default);
}
