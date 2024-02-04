namespace RecipeSocialMediaAPI.Application.Contracts.Messages;
public record NewMessageContract(string ConversationId, string SenderId, string? RecipientId, string Text, List<string> RecipeIds, List<string> ImageURLs, string SentDate, string? MessageRepliedTo);
