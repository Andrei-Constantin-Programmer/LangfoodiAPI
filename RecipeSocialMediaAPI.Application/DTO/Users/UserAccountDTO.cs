namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record UserAccountDto(
    string Id,
    string Handler,
    string UserName,
    List<string> PinnedConversationIds,
    List<string> BlockedConnectionIds,
    string? ProfileImageId = null,
    DateTimeOffset? AccountCreationDate = null
);
