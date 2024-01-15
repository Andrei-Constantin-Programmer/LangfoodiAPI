using RecipeSocialMediaAPI.Application.DTO.Users;

namespace RecipeSocialMediaAPI.Application.DTO.Recipes;

public record RecipeDetailedDTO(
    string Id,
    string Title,
    string Description,
    UserAccountDTO Chef,
    ISet<string> Tags,
    List<IngredientDTO> Ingredients,
    Stack<RecipeStepDTO> RecipeSteps,
    string? ThumbnailId = null,
    int? NumberOfServings = null,
    int? CookingTime = null,
    int? KiloCalories = null,
    ServingSizeDTO? ServingSize = null,
    DateTimeOffset? CreationDate = null,
    DateTimeOffset? LastUpdatedDate = null
);