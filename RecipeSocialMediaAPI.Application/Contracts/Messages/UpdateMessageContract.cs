namespace RecipeSocialMediaAPI.Application.Contracts.Messages;

public record UpdateMessageContract(string Id, string? Text, List<string>? RecipeIds, List<string>? ImageURLs);