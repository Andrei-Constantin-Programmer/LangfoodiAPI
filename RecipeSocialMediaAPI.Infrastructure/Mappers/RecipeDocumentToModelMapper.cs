﻿using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers;

public class RecipeDocumentToModelMapper : IRecipeDocumentToModelMapper
{
    public Recipe MapRecipeDocumentToRecipe(RecipeDocument recipeDocument, IUserAccount chef)
    {
        if (recipeDocument.Id == null)
        {
            throw new ArgumentException("Cannot map Recipe Document with null ID to Recipe");
        }

        var (servingSizeQuantity, unitOfMeasurement) = recipeDocument.ServingSize ?? default;

        return new(
            id: recipeDocument.Id,
            title: recipeDocument.Title,
            description: recipeDocument.Description,
            creationDate: recipeDocument.CreationDate,
            lastUpdatedDate: recipeDocument.LastUpdatedDate,
            tags: recipeDocument.Tags.ToHashSet(),
            chef: chef,
            thumbnailId: recipeDocument.ThumbnailId,
            recipeGuide: new RecipeGuide(
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
                kiloCalories: recipeDocument.KiloCalories,
                servingSize: recipeDocument.ServingSize is not null
                    ? new ServingSize(servingSizeQuantity, unitOfMeasurement)
                    : null
                )
        );
    }
}