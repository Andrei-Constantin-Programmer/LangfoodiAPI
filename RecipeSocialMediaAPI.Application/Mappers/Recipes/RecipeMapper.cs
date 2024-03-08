﻿using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Recipes;

namespace RecipeSocialMediaAPI.Application.Mappers.Recipes;

public class RecipeMapper : IRecipeMapper
{
    private readonly IUserMapper _userMapper;

    public RecipeMapper(IUserMapper userMapper)
    {
        _userMapper = userMapper;
    }

    public ServingSize MapServingSizeDtoToServingSize(ServingSizeDTO servingSizeDTO)
    {
        return new(servingSizeDTO.Quantity, servingSizeDTO.UnitOfMeasurement);
    }

    public ServingSizeDTO MapServingSizeToServingSizeDto(ServingSize servingSize)
    {
        return new ServingSizeDTO(servingSize.Quantity, servingSize.UnitOfMeasurement);
    }

    public Ingredient MapIngredientDtoToIngredient(IngredientDTO ingredientDTO)
    {
        return new(ingredientDTO.Name, ingredientDTO.Quantity, ingredientDTO.UnitOfMeasurement);
    }

    public IngredientDTO MapIngredientToIngredientDto(Ingredient ingredient)
    {
        return new IngredientDTO(ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement);
    }

    public RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDTO recipeStepDTO)
    {
        return new(recipeStepDTO.Text, recipeStepDTO.ImageUrl is null ? null : new RecipeImage(recipeStepDTO.ImageUrl));
    }

    public RecipeStepDTO MapRecipeStepToRecipeStepDto(RecipeStep recipeStep)
    {
        return new RecipeStepDTO(recipeStep.Text, recipeStep.Image?.ImageUrl);
    }

    public RecipeDetailedDTO MapRecipeAggregateToRecipeDetailedDto(Recipe recipeAggregate)
    {
        return new RecipeDetailedDTO(
            Id: recipeAggregate.Id,
            Title: recipeAggregate.Title,
            Description: recipeAggregate.Description,
            Chef: _userMapper.MapUserAccountToUserAccountDto(recipeAggregate.Chef),
            Tags: recipeAggregate.Tags,
            Ingredients: recipeAggregate.Guide.Ingredients
                .Select(MapIngredientToIngredientDto)
                .ToList(),
            RecipeSteps: new Stack<RecipeStepDTO>(recipeAggregate.Guide.Steps
                .Select(MapRecipeStepToRecipeStepDto)),
            KiloCalories: recipeAggregate.Guide.KiloCalories,
            NumberOfServings: recipeAggregate.Guide.NumberOfServings,
            CookingTime: recipeAggregate.Guide.CookingTimeInSeconds,
            CreationDate: recipeAggregate.CreationDate,
            LastUpdatedDate: recipeAggregate.LastUpdatedDate,
            ThumbnailId: recipeAggregate.ThumbnailId,
            ServingSize: recipeAggregate.Guide.ServingSize is not null
                ? MapServingSizeToServingSizeDto(recipeAggregate.Guide.ServingSize)
                : null
        );
    }

    public RecipeDTO MapRecipeAggregateToRecipeDto(Recipe recipeAggregate)
    {
        return new RecipeDTO(
            Id: recipeAggregate.Id,
            Title: recipeAggregate.Title,
            Description: recipeAggregate.Description,
            ChefUsername: recipeAggregate.Chef.UserName,
            Tags: recipeAggregate.Tags,
            KiloCalories: recipeAggregate.Guide.KiloCalories,
            NumberOfServings: recipeAggregate.Guide.NumberOfServings,
            CookingTime: recipeAggregate.Guide.CookingTimeInSeconds,
            CreationDate: recipeAggregate.CreationDate,
            LastUpdatedDate: recipeAggregate.LastUpdatedDate,
            ThumbnailId: recipeAggregate.ThumbnailId
        );
    }

    public RecipePreviewDTO MapRecipeAggregateToRecipePreviewDto(Recipe recipeAggregate)
    {
        return new RecipePreviewDTO(
            recipeAggregate.Id,
            recipeAggregate.Title,
            recipeAggregate.ThumbnailId
        );
    }
}
