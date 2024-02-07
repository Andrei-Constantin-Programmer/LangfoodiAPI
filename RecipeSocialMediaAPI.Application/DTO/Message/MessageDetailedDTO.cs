using RecipeSocialMediaAPI.Application.DTO.Recipes;

namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record MessageDetailedDTO
(
    string Id,
    string SenderId,
    string SenderName,
    List<string> SeenByUserIds,
    DateTimeOffset? SentDate = null,
    DateTimeOffset? UpdatedDate = null,
    MessageDetailedDTO? RepliedToMessage = null,
    string? TextContent = null,
    List<string>? ImageURLs = null,
    List<RecipeDTO>? Recipes = null
);
