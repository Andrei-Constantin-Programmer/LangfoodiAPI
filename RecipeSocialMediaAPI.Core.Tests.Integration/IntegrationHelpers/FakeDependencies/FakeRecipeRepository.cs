using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeRecipeRepository : IRecipeQueryRepository, IRecipePersistenceRepository
{
    private readonly List<RecipeAggregate> _collection;

    public FakeRecipeRepository()
    {
        _collection = new List<RecipeAggregate>();
    }

    public async Task<RecipeAggregate> CreateRecipeAsync(string title, Recipe recipe, string description, IUserAccount chef, ISet<string> tags, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate, string? thumbnailId, CancellationToken cancellationToken = default)
    {
        var id = _collection.Count.ToString();
        RecipeAggregate newRecipe = new(
            id, title, recipe, description, chef, 
            creationDate, lastUpdatedDate, tags,
            thumbnailId
        );
        _collection.Add(newRecipe);

        return await Task.FromResult(newRecipe);
    }

    public async Task<bool> DeleteRecipeAsync(RecipeAggregate recipe, CancellationToken cancellationToken = default) 
        => await DeleteRecipeAsync(recipe.Id, cancellationToken);

    public async Task<bool> DeleteRecipeAsync(string id, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.RemoveAll(x => x.Id == id) > 0);

    public async Task<RecipeAggregate?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Find(x => x.Id == id));

    public async Task<IEnumerable<RecipeAggregate>> GetRecipesByChefAsync(IUserAccount? user, CancellationToken cancellationToken = default) => await GetRecipesByChefIdAsync(user!.Id, cancellationToken);

    public async Task<IEnumerable<RecipeAggregate>> GetRecipesByChefIdAsync(string chefId, CancellationToken cancellationToken = default) => await Task.FromResult(_collection.FindAll(x => x.Chef.Id == chefId));

    public async Task<IEnumerable<RecipeAggregate>> GetRecipesByChefNameAsync(string chefName, CancellationToken cancellationToken = default) => await Task.FromResult(_collection.FindAll(x => x.Chef.UserName == chefName));

    public async Task<bool> UpdateRecipeAsync(RecipeAggregate recipe, CancellationToken cancellationToken = default)
    {
        RecipeAggregate? existingRecipe = _collection.FirstOrDefault(x => x.Id == recipe.Id);
        if (existingRecipe is null)
        {
            return false;
        }

        RecipeAggregate updatedRecipe = new(
            existingRecipe.Id, recipe.Title,
            recipe.Recipe, recipe.Description,
            existingRecipe.Chef, existingRecipe.CreationDate,
            existingRecipe.LastUpdatedDate, existingRecipe.Tags,
            existingRecipe.ThumbnailId
        );

        _collection.Remove(existingRecipe);
        _collection.Add(updatedRecipe);

        return await Task.FromResult(true);
    }
}
