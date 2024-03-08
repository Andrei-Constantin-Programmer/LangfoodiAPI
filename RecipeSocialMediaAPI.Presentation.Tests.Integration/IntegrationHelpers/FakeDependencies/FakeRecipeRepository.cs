using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeRecipeRepository : IRecipeQueryRepository, IRecipePersistenceRepository
{
    private readonly List<Recipe> _collection;

    public FakeRecipeRepository()
    {
        _collection = new List<Recipe>();
    }

    public async Task<Recipe> CreateRecipeAsync(string title, RecipeGuide recipe, string description, IUserAccount chef, ISet<string> tags, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate, string? thumbnailId, CancellationToken cancellationToken = default)
    {
        var id = _collection.Count.ToString();
        Recipe newRecipe = new(
            id, title, recipe, description, chef, 
            creationDate, lastUpdatedDate, tags,
            thumbnailId
        );
        _collection.Add(newRecipe);

        return await Task.FromResult(newRecipe);
    }

    public async Task<bool> DeleteRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default) 
        => await DeleteRecipeAsync(recipe.Id, cancellationToken);

    public async Task<bool> DeleteRecipeAsync(string id, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.RemoveAll(x => x.Id == id) > 0);

    public async Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Find(x => x.Id == id));

    public async Task<IEnumerable<Recipe>> GetRecipesByChefAsync(IUserAccount? user, CancellationToken cancellationToken = default) => await GetRecipesByChefIdAsync(user!.Id, cancellationToken);

    public async Task<IEnumerable<Recipe>> GetRecipesByChefIdAsync(string chefId, CancellationToken cancellationToken = default) => await Task.FromResult(_collection.FindAll(x => x.Chef.Id == chefId));

    public async Task<IEnumerable<Recipe>> GetRecipesByChefNameAsync(string chefName, CancellationToken cancellationToken = default) => await Task.FromResult(_collection.FindAll(x => x.Chef.UserName == chefName));

    public async Task<bool> UpdateRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        Recipe? existingRecipe = _collection.FirstOrDefault(x => x.Id == recipe.Id);
        if (existingRecipe is null)
        {
            return false;
        }

        Recipe updatedRecipe = new(
            existingRecipe.Id, recipe.Title,
            recipe.Guide, recipe.Description,
            existingRecipe.Chef, existingRecipe.CreationDate,
            existingRecipe.LastUpdatedDate, existingRecipe.Tags,
            existingRecipe.ThumbnailId
        );

        _collection.Remove(existingRecipe);
        _collection.Add(updatedRecipe);

        return await Task.FromResult(true);
    }
}
