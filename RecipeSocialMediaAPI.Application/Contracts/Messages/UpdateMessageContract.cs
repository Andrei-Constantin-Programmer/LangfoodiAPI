namespace RecipeSocialMediaAPI.Application.Contracts.Messages;

public record UpdateMessageContract(string Id, string? Text, List<string>? NewRecipeIds, List<string>? NewImageURLs);