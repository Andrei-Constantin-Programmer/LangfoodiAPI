﻿namespace RecipeSocialMediaAPI.Application.DTO.Users;

public record UserAccountDTO(
    string Id,
    string Handler,
    string UserName,
    List<string> PinnedConversationIds,
    List<string> BlockedConnectionIds,
    string? ProfileImageId = null,
    DateTimeOffset? AccountCreationDate = null
);