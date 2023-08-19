using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.DataAccess.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoCollectionWrapper<RecipeDocument> _recipeCollection;
    private readonly IRecipeDocumentToModelMapper _mapper;

    public RecipeRepository(IRecipeDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _recipeCollection = mongoCollectionFactory.CreateCollection<RecipeDocument>();
    }

    public Recipe? GetRecipeById(string id) => throw new NotImplementedException();
    public IEnumerable<Recipe> GetRecipesByChef(string chefId) => throw new NotImplementedException();
    public Recipe CreateRecipe(Recipe recipe) => throw new NotImplementedException();
    public bool UpdateRecipe(Recipe recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(Recipe recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(string id) => throw new NotImplementedException();
}
