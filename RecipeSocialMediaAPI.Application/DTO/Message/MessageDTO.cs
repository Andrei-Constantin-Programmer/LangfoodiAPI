namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record MessageDTO(
    string Id,
    string SenderId,
    DateTimeOffset? SentDate = null,
    DateTimeOffset? UpdatedDate = null,
    string? RepliedToMessageId = null,
    string? TextContent = null,
    List<string>? ImageURLs = null,
    List<string>? RecipeIds = null
);