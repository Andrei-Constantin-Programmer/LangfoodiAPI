using RecipeSocialMediaAPI.Application.DTO.Recipes;

namespace RecipeSocialMediaAPI.Application.Contracts.Recipes;

public record NewRecipeContract(
    string Title,
    string Description,
    string ChefId,
    ISet<string> Tags,
    List<IngredientDTO> Ingredients,
    Stack<RecipeStepDTO> RecipeSteps,
    string? ThumbnailId = null,
    int? NumberOfServings = null,
    int? CookingTime = null,
    int? KiloCalories = null,
    ServingSizeDTO? ServingSize = null);
