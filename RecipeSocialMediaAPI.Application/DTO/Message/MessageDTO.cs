using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Users;

namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record MessageDTO(
    string Id,
    UserPreviewForMessageDTO UserPreview,
    List<string> SeenByUserIds,
    DateTimeOffset? SentDate = null,
    DateTimeOffset? UpdatedDate = null,
    string? RepliedToMessageId = null,
    string? TextContent = null,
    List<string>? ImageURLs = null,
    List<RecipePreviewDTO>? Recipes = null
);