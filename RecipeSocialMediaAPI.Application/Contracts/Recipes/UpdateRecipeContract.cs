using RecipeSocialMediaAPI.Application.DTO.Recipes;

namespace RecipeSocialMediaAPI.Application.Contracts.Recipes;

public record UpdateRecipeContract(
    string Id,
    string Title,
    string Description,
    ISet<string> Tags,
    List<IngredientDTO> Ingredients,
    Stack<RecipeStepDTO> RecipeSteps,
    string? ThumbnailId = null,
    int? NumberOfServings = null,
    int? CookingTime = null,
    int? KiloCalories = null,
    ServingSizeDTO? ServingSize = null
);