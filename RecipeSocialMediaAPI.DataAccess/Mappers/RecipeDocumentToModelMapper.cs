using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class RecipeDocumentToModelMapper : IRecipeDocumentToModelMapper
{
    public RecipeAggregate MapRecipeDocumentToRecipeAggregate(RecipeDocument recipeDocument, IUserAccount chef)
    {
        if (recipeDocument.Id == null)
        {
            throw new ArgumentException("Cannot map Recipe Document with null ID to Recipe Aggregate");
        }

        return new(
            id: recipeDocument.Id,
            title: recipeDocument.Title,
            description: recipeDocument.Description,
            creationDate: recipeDocument.CreationDate,
            lastUpdatedDate: recipeDocument.LastUpdatedDate,
            labels: recipeDocument.Labels.ToHashSet(),
            chef: chef,
            recipe: new Recipe(
                recipeDocument.Ingredients
                    .Select(x => new Ingredient(x.Name, x.Quantity, x.UnitOfMeasurement))
                    .ToList(),
                new Stack<RecipeStep>(
                    recipeDocument.Steps
                    .Select(x => new RecipeStep(
                        x.Text,
                        x.ImageLink is null ? null : new RecipeImage(x.ImageLink)))),
                numberOfServings: recipeDocument.NumberOfServings,
                cookingTimeInSeconds: recipeDocument.CookingTimeInSeconds,
                kiloCalories: recipeDocument.KiloCalories
                )
            );
    }
}