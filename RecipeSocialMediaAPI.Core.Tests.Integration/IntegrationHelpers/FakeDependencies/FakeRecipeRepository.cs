using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeRecipeRepository : IRecipeRepository
{
    private readonly List<Recipe> _recipes;

    public FakeRecipeRepository() 
    {
        _recipes = new List<Recipe>();
    }

    public RecipeAggregate CreateRecipe(string title, Recipe recipe, string description, User chef, ISet<string> labels, int? numberOfServings, int? cookingTime, int? kiloCalories, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate) => throw new NotImplementedException();
    public bool DeleteRecipe(RecipeAggregate recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(string id) => throw new NotImplementedException();
    public RecipeAggregate? GetRecipeById(string id) => throw new NotImplementedException();
    public IEnumerable<RecipeAggregate> GetRecipesByChef(User? user) => throw new NotImplementedException();
    public IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId) => throw new NotImplementedException();
    public IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName) => throw new NotImplementedException();
    public bool UpdateRecipe(RecipeAggregate recipe) => throw new NotImplementedException();
}
