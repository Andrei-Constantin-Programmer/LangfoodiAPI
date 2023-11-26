namespace RecipeSocialMediaAPI.Application.Contracts.Messages;

public record UpdateConnectionContract(string UserId1, string UserId2, string NewConnectionStatus);
