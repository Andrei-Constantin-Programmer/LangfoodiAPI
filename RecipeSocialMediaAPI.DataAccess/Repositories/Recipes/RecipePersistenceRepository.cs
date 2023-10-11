using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Recipes;

public class RecipePersistenceRepository : IRecipePersistenceRepository
{
    private readonly IRecipeDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<RecipeDocument> _recipeCollection;

    public RecipePersistenceRepository(IRecipeDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _recipeCollection = mongoCollectionFactory.CreateCollection<RecipeDocument>();
    }

    public RecipeAggregate CreateRecipe(string title, Recipe recipe, string description, IUserAccount chef, ISet<string> labels, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate)
    {
        var recipeDocument = _recipeCollection
            .Insert(new RecipeDocument()
            {
                Title = title,
                Ingredients = recipe.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps = recipe.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                Description = description,
                ChefId = chef.Id,
                CreationDate = creationDate,
                LastUpdatedDate = lastUpdatedDate,
                Labels = labels.ToList(),
                NumberOfServings = recipe.NumberOfServings,
                CookingTimeInSeconds = recipe.CookingTimeInSeconds,
                KiloCalories = recipe.KiloCalories
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
                NumberOfServings = recipe.Recipe.NumberOfServings,
                CookingTimeInSeconds = recipe.Recipe.CookingTimeInSeconds,
                KiloCalories = recipe.Recipe.KiloCalories,
                CreationDate = recipe.CreationDate,
                LastUpdatedDate = recipe.LastUpdatedDate,
                Labels = recipe.Labels.ToList(),
            },
            doc => doc.Id == recipe.Id
        );

    public bool DeleteRecipe(RecipeAggregate recipe) => DeleteRecipe(recipe.Id);

    public bool DeleteRecipe(string id) => _recipeCollection.Delete(recipeDoc => recipeDoc.Id == id);
}
