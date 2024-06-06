using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Recipes;

public class RecipePersistenceRepository : IRecipePersistenceRepository
{
    private readonly IRecipeDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<RecipeDocument> _recipeCollection;

    public RecipePersistenceRepository(IRecipeDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _recipeCollection = mongoCollectionFactory.CreateCollection<RecipeDocument>();
    }

    public async Task<Recipe> CreateRecipeAsync(
        string title,
        RecipeGuide recipeGuide,
        string description,
        IUserAccount chef,
        ISet<string> tags,
        DateTimeOffset creationDate,
        DateTimeOffset lastUpdatedDate,
        string? thumbnailId,
        CancellationToken cancellationToken = default)
    {
        var recipeDocument = await _recipeCollection
            .InsertAsync(new RecipeDocument(
                Title: title,
                Ingredients: recipeGuide.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps: recipeGuide.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                Description: description,
                ChefId: chef.Id,
                ThumbnailId: thumbnailId,
                CreationDate: creationDate,
                LastUpdatedDate: lastUpdatedDate,
                Tags: tags.ToList(),
                NumberOfServings: recipeGuide.NumberOfServings,
                CookingTimeInSeconds: recipeGuide.CookingTimeInSeconds,
                KiloCalories: recipeGuide.KiloCalories,
                ServingSize: recipeGuide.ServingSize is not null ? (recipeGuide.ServingSize.Quantity, recipeGuide.ServingSize.UnitOfMeasurement) : null
            ), cancellationToken);

        return _mapper.MapRecipeDocumentToRecipe(recipeDocument, chef);
    }

    public async Task<bool> UpdateRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default) => await
        _recipeCollection.UpdateAsync(
            new RecipeDocument(
                Id: recipe.Id,
                Title: recipe.Title,
                Ingredients: recipe.Guide.Ingredients.Select(ingredient => (ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement)).ToList(),
                Steps: recipe.Guide.Steps.Select(step => (step.Text, step.Image?.ImageUrl)).ToList(),
                Description: recipe.Description,
                ChefId: recipe.Chef.Id,
                ThumbnailId: recipe.ThumbnailId,
                NumberOfServings: recipe.Guide.NumberOfServings,
                CookingTimeInSeconds: recipe.Guide.CookingTimeInSeconds,
                KiloCalories: recipe.Guide.KiloCalories,
                CreationDate: recipe.CreationDate,
                LastUpdatedDate: recipe.LastUpdatedDate,
                Tags: recipe.Tags.ToList(),
                ServingSize: recipe.Guide.ServingSize is not null ? (recipe.Guide.ServingSize.Quantity, recipe.Guide.ServingSize.UnitOfMeasurement) : null),
            doc => doc.Id == recipe.Id,
            cancellationToken
        );

    public async Task<bool> DeleteRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default)
        => await DeleteRecipeAsync(recipe.Id, cancellationToken);

    public async Task<bool> DeleteRecipeAsync(string id, CancellationToken cancellationToken = default)
        => await _recipeCollection.DeleteAsync(recipeDoc => recipeDoc.Id == id, cancellationToken);
}
