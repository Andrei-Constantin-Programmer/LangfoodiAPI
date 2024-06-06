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

    public ServingSize MapServingSizeDtoToServingSize(ServingSizeDto servingSizeDTO)
    {
        return new(servingSizeDTO.Quantity, servingSizeDTO.UnitOfMeasurement);
    }

    public ServingSizeDto MapServingSizeToServingSizeDto(ServingSize servingSize)
    {
        return new ServingSizeDto(servingSize.Quantity, servingSize.UnitOfMeasurement);
    }

    public Ingredient MapIngredientDtoToIngredient(IngredientDto ingredientDTO)
    {
        return new(ingredientDTO.Name, ingredientDTO.Quantity, ingredientDTO.UnitOfMeasurement);
    }

    public IngredientDto MapIngredientToIngredientDto(Ingredient ingredient)
    {
        return new IngredientDto(ingredient.Name, ingredient.Quantity, ingredient.UnitOfMeasurement);
    }

    public RecipeStep MapRecipeStepDtoToRecipeStep(RecipeStepDto recipeStepDTO)
    {
        return new(recipeStepDTO.Text, recipeStepDTO.ImageUrl is null ? null : new RecipeImage(recipeStepDTO.ImageUrl));
    }

    public RecipeStepDto MapRecipeStepToRecipeStepDto(RecipeStep recipeStep)
    {
        return new RecipeStepDto(recipeStep.Text, recipeStep.Image?.ImageUrl);
    }

    public RecipeDetailedDto MapRecipeToRecipeDetailedDto(Recipe recipe)
    {
        return new RecipeDetailedDto(
            Id: recipe.Id,
            Title: recipe.Title,
            Description: recipe.Description,
            Chef: _userMapper.MapUserAccountToUserAccountDto(recipe.Chef),
            Tags: recipe.Tags,
            Ingredients: recipe.Guide.Ingredients
                .Select(MapIngredientToIngredientDto)
                .ToList(),
            RecipeSteps: new Stack<RecipeStepDto>(recipe.Guide.Steps
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

    public RecipeDto MapRecipeToRecipeDto(Recipe recipe)
    {
        return new RecipeDto(
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

    public RecipePreviewDto MapRecipeToRecipePreviewDto(Recipe recipe)
    {
        return new RecipePreviewDto(
            recipe.Id,
            recipe.Title,
            recipe.ThumbnailId
        );
    }
}
