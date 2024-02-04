namespace RecipeSocialMediaAPI.Application.Contracts.Messages;

public record UpdateGroupContract(string GroupId, string GroupName, string GroupDescription, List<string> UserIds);