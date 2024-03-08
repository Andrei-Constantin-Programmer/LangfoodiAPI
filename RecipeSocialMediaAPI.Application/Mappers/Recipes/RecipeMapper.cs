using RecipeSocialMediaAPI.Application.DTO.Recipes;
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

    public RecipeDetailedDTO MapRecipeToRecipeDetailedDto(Recipe recipe)
    {
        return new RecipeDetailedDTO(
            Id: recipe.Id,
            Title: recipe.Title,
            Description: recipe.Description,
            Chef: _userMapper.MapUserAccountToUserAccountDto(recipe.Chef),
            Tags: recipe.Tags,
            Ingredients: recipe.Guide.Ingredients
                .Select(MapIngredientToIngredientDto)
                .ToList(),
            RecipeSteps: new Stack<RecipeStepDTO>(recipe.Guide.Steps
                .Select(MapRecipeStepToRecipeStepDto)),
            KiloCalories: recipe.Guide.KiloCalories,
            NumberOfServings: recipe.Guide.NumberOfServings,
            CookingTime: recipe.Guide.CookingTimeInSeconds,
            CreationDate: recipe.CreationDate,
            LastUpdatedDate: recipe.LastUpdatedDate,
            ThumbnailId: recipe.ThumbnailId,
            ServingSize: recipe.Guide.ServingSize is not null
                ? MapServingSizeToServingSizeDto(recipe.Guide.ServingSize)
                : null
        );
    }

    public RecipeDTO MapRecipeToRecipeDto(Recipe recipe)
    {
        return new RecipeDTO(
            Id: recipe.Id,
            Title: recipe.Title,
            Description: recipe.Description,
            ChefUsername: recipe.Chef.UserName,
            Tags: recipe.Tags,
            KiloCalories: recipe.Guide.KiloCalories,
            NumberOfServings: recipe.Guide.NumberOfServings,
            CookingTime: recipe.Guide.CookingTimeInSeconds,
            CreationDate: recipe.CreationDate,
            LastUpdatedDate: recipe.LastUpdatedDate,
            ThumbnailId: recipe.ThumbnailId
        );
    }

    public RecipePreviewDTO MapRecipeToRecipePreviewDto(Recipe recipe)
    {
        return new RecipePreviewDTO(
            recipe.Id,
            recipe.Title,
            recipe.ThumbnailId
        );
    }
}
