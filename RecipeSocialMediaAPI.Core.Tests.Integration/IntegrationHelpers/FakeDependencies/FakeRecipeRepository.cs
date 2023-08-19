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

    public async Task<IEnumerable<Recipe>> GetRecipes()
    {
        return await Task.FromResult(_recipes);
    }

    public Task<Recipe?> GetRecipeById(int id) => throw new NotImplementedException();
    public Recipe? GetRecipeById(string id) => throw new NotImplementedException();
    public IEnumerable<Recipe> GetRecipesByChef(string chefId) => throw new NotImplementedException();
    Recipe IRecipeRepository.CreateRecipe(Recipe recipe) => throw new NotImplementedException();
    public bool UpdateRecipe(Recipe recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(Recipe recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(string id) => throw new NotImplementedException();
}
