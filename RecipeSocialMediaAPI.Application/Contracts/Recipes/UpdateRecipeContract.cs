using RecipeSocialMediaAPI.Application.DTO.Recipes;

namespace RecipeSocialMediaAPI.Application.Contracts.Recipes;

public record UpdateRecipeContract(
    string Id,
    string Title,
    string Description,
    ISet<string> Tags,
    List<IngredientDto> Ingredients,
    Stack<RecipeStepDto> RecipeSteps,
    string? ThumbnailId = null,
    int? NumberOfServings = null,
    int? CookingTime = null,
    int? KiloCalories = null,
    ServingSizeDto? ServingSize = null
);