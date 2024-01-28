namespace RecipeSocialMediaAPI.Application.DTO.Recipes;
public record RecipePreviewDTO(
    string Id,
    string Title,
    string? ThumbnailId
);
