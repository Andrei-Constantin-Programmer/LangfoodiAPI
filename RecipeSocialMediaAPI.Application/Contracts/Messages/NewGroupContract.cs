namespace RecipeSocialMediaAPI.Application.Contracts.Messages;

public record NewGroupContract(string Name, string Description, List<string> UserIds);