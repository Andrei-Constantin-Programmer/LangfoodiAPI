using RecipeSocialMediaAPI.Application.DTO.Users;

namespace RecipeSocialMediaAPI.Application.DTO.Recipes;

public record RecipeDetailedDto(
    string Id,
    string Title,
    string Description,
    UserAccountDto Chef,
    ISet<string> Tags,
    List<IngredientDto> Ingredients,
    Stack<RecipeStepDto> RecipeSteps,
    string? ThumbnailId = null,
    int? NumberOfServings = null,
    int? CookingTime = null,
    int? KiloCalories = null,
    ServingSizeDto? ServingSize = null,
    DateTimeOffset? CreationDate = null,
    DateTimeOffset? LastUpdatedDate = null
);