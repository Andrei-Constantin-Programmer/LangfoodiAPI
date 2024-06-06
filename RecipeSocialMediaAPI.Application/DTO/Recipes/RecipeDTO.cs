namespace RecipeSocialMediaAPI.Application.DTO.Recipes;

public record RecipeDto(
    string Id,
    string Title,
    string Description,
    string ChefUsername,
    ISet<string> Tags,
    string? ThumbnailId = null,
    int? NumberOfServings = null,
    int? CookingTime = null,
    int? KiloCalories = null,
    DateTimeOffset? CreationDate = null,
    DateTimeOffset? LastUpdatedDate = null
);
