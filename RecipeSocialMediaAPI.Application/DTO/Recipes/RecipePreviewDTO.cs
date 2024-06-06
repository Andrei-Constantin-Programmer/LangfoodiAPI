namespace RecipeSocialMediaAPI.Application.DTO.Recipes;

public record RecipePreviewDto(
    string Id,
    string Title,
    string? ThumbnailId
);
