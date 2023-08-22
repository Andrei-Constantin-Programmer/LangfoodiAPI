using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Reflection.Emit;

namespace RecipeSocialMediaAPI.DataAccess.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly IRecipeDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<RecipeDocument> _recipeCollection;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RecipeRepository> _logger;

    public RecipeRepository(IRecipeDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory, IUserRepository userRepository, ILogger<RecipeRepository> logger)
    {
        _mapper = mapper;
        _recipeCollection = mongoCollectionFactory.CreateCollection<RecipeDocument>();
        _userRepository = userRepository;
        _logger = logger;
    }

    public RecipeAggregate? GetRecipeById(string id)
    {
        RecipeDocument? recipeDocument = _recipeCollection
            .Find(recipeDoc => recipeDoc.Id == id);

        if (recipeDocument is null)
        {
            return null;
        }

        User? chef = _userRepository.GetUserById(recipeDocument.ChefId);

        if (chef is null)
        {
            _logger.LogWarning("The chef with id {ChefId} was not found for recipe with id {RecipeId}", recipeDocument.ChefId, recipeDocument.Id);
            return null;
        }

        return _mapper.MapRecipeDocumentToRecipeAggregate(recipeDocument, chef);
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChef(string chefId)
    {
        User? chef = _userRepository.GetUserById(chefId);

        if (chef is null)
        {
            return Enumerable.Empty<RecipeAggregate>();
        }

        var recipes = _recipeCollection
            .GetAll(recipeDoc => recipeDoc.ChefId == chefId);

        return recipes.Count == 0
            ? Enumerable.Empty<RecipeAggregate>()
            : recipes.Select(recipeDoc => _mapper.MapRecipeDocumentToRecipeAggregate(recipeDoc, chef));
    }

    public RecipeAggregate CreateRecipe(string title, Recipe recipe, string shortDescription, string longDescription, User chef, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate, ISet<string> labels)
    {
        var recipeDocument = _recipeCollection
            .Insert(new RecipeDocument()
            {
                Title = title,
                Ingredients = recipe.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps = recipe.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                ShortDescription = shortDescription,
                LongDescription = longDescription,
                ChefId = chef.Id,
                CreationDate = creationDate,
                LastUpdatedDate = lastUpdatedDate,
                Labels = labels.ToList()
            });
        
        return _mapper.MapRecipeDocumentToRecipeAggregate(recipeDocument, chef);
    }

    public bool UpdateRecipe(RecipeAggregate recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(RecipeAggregate recipe) => throw new NotImplementedException();
    public bool DeleteRecipe(string id) => throw new NotImplementedException();
}
