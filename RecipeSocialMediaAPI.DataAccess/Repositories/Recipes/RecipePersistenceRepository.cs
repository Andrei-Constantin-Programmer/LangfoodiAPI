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

    public async Task<RecipeAggregate> CreateRecipe(string title, Recipe recipe, string description, IUserAccount chef, ISet<string> tags, DateTimeOffset creationDate, DateTimeOffset lastUpdatedDate, string? thumbnailId, CancellationToken cancellationToken = default)
    {
        var recipeDocument = await _recipeCollection
            .Insert(new RecipeDocument(
                Title: title,
                Ingredients: recipe.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps: recipe.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                Description: description,
                ChefId: chef.Id,
                ThumbnailId: thumbnailId,
                CreationDate: creationDate,
                LastUpdatedDate: lastUpdatedDate,
                Tags: tags.ToList(),
                NumberOfServings: recipe.NumberOfServings,
                CookingTimeInSeconds: recipe.CookingTimeInSeconds,
                KiloCalories: recipe.KiloCalories,
                ServingSize: recipe.ServingSize is not null ? (recipe.ServingSize.Quantity, recipe.ServingSize.UnitOfMeasurement) : null
            ), cancellationToken);

        return _mapper.MapRecipeDocumentToRecipeAggregate(recipeDocument, chef);
    }

    public async Task<bool> UpdateRecipe(RecipeAggregate recipe, CancellationToken cancellationToken = default) => await 
        _recipeCollection.UpdateRecord(
            new RecipeDocument(
                Id: recipe.Id,
                Title: recipe.Title,
                Ingredients: recipe.Recipe.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps: recipe.Recipe.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                Description: recipe.Description,
                ChefId: recipe.Chef.Id,
                ThumbnailId: recipe.ThumbnailId,
                NumberOfServings: recipe.Recipe.NumberOfServings,
                CookingTimeInSeconds: recipe.Recipe.CookingTimeInSeconds,
                KiloCalories: recipe.Recipe.KiloCalories,
                CreationDate: recipe.CreationDate,
                LastUpdatedDate: recipe.LastUpdatedDate,
                Tags: recipe.Tags.ToList(),
                ServingSize: recipe.Recipe.ServingSize is not null ? (recipe.Recipe.ServingSize.Quantity, recipe.Recipe.ServingSize.UnitOfMeasurement) : null),
            doc => doc.Id == recipe.Id,
            cancellationToken
        );

    public bool DeleteRecipe(RecipeAggregate recipe) => DeleteRecipe(recipe.Id);

    public bool DeleteRecipe(string id) => _recipeCollection.Delete(recipeDoc => recipeDoc.Id == id);
}
