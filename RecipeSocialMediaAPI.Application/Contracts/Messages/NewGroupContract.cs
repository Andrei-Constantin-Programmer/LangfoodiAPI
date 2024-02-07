namespace RecipeSocialMediaAPI.Application.Contracts.Messages;

public record NewGroupContract(string Id, string Name, string Description, List<string> UserIds);