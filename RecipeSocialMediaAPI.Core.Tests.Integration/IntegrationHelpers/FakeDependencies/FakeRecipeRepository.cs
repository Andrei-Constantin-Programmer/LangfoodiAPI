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

    public RecipeAggregate CreateRecipe(RecipeAggregate recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(RecipeAggregate recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(string id) => throw new NotImplementedException();
    public RecipeAggregate? GetRecipeById(string id) => throw new NotImplementedException();
    public IEnumerable<RecipeAggregate> GetRecipesByChef(string chefId) => throw new NotImplementedException();
    public bool UpdateRecipe(RecipeAggregate recipe) => throw new NotImplementedException();
}
