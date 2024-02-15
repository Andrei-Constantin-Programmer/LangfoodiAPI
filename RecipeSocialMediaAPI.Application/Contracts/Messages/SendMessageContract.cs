namespace RecipeSocialMediaAPI.Application.Contracts.Messages;

public record SendMessageContract(
    string ConversationId,
    string SenderId,
    string? Text,
    List<string>? RecipeIds,
    List<string>? ImageURLs,
    string? MessageRepliedToId
);
