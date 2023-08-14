using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeRecipeRepository : IRecipeRepository
{
    private readonly List<Recipe> _recipes;

    public FakeRecipeRepository() 
    {
        _recipes = new List<Recipe>();
    }

    public Task CreateRecipe(Recipe recipe)
    {
        _recipes.Add(recipe);

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Recipe>> GetAllRecipes()
    {
        return await Task.FromResult(_recipes);
    }

    public Task<Recipe?> GetRecipeById(int id) => throw new NotImplementedException();

    //public async Task<Recipe?> GetRecipeById(int id)
    //{
    //    return await Task.FromResult(_recipes.SingleOrDefault(r => r.Id == id));
    //}
}
