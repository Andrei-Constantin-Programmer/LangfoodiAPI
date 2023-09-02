using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;

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

    public IEnumerable<RecipeAggregate> GetRecipesByChef(User? chef)
    {
        if (chef is null)
        {
            return Enumerable.Empty<RecipeAggregate>();
        }

        var recipes = _recipeCollection
            .GetAll(recipeDoc => recipeDoc.ChefId == chef.Id);

        return recipes.Count == 0
            ? Enumerable.Empty<RecipeAggregate>()
            : recipes.Select(recipeDoc => _mapper.MapRecipeDocumentToRecipeAggregate(recipeDoc, chef));
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChefId(string chefId)
    {
        User? chef = _userRepository.GetUserById(chefId);
        return GetRecipesByChef(chef);
    }

    public IEnumerable<RecipeAggregate> GetRecipesByChefName(string chefName)
    {
        User? chef = _userRepository.GetUserByUsername(chefName);
        return GetRecipesByChef(chef);
    }

    public RecipeAggregate CreateRecipe(string title, Recipe recipe, string description, User chef, ISet<string> labels, int? numberOfServings, int? cookingTime, int? kiloCalories, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate)
    {
        var recipeDocument = _recipeCollection
            .Insert(new RecipeDocument()
            {
                Title = title,
                Ingredients = recipe.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps = recipe.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                Description = description,
                ChefId = chef.Id,
                NumberOfServings = numberOfServings,
                CookingTimeInSeconds = cookingTime,
                KiloCalories = kiloCalories,
                CreationDate = creationDate,
                LastUpdatedDate = lastUpdatedDate,
                Labels = labels.ToList()
            });
        
        return _mapper.MapRecipeDocumentToRecipeAggregate(recipeDocument, chef);
    }

    public bool UpdateRecipe(RecipeAggregate recipe) => _recipeCollection.UpdateRecord(
            new RecipeDocument()
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Ingredients = recipe.Recipe.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps = recipe.Recipe.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                Description = recipe.Description,
                ChefId = recipe.Chef.Id,
                CreationDate = recipe.CreationDate,
                LastUpdatedDate = recipe.LastUpdatedDate,
                Labels = recipe.Labels.ToList()
            },
            doc => doc.Id == recipe.Id
        );

    public bool DeleteRecipe(RecipeAggregate recipe) => DeleteRecipe(recipe.Id);

    public bool DeleteRecipe(string id) => _recipeCollection.Delete(recipeDoc => recipeDoc.Id == id);
}
