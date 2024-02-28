namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record ConversationDTO(
    string Id,
    string ConnectionOrGroupId,
    bool IsGroup,
    string Name,
    string? ThumbnailId,
    MessageDTO? LastMessage,
    List<string> UserIds,
    int MessagesUnseen = 0
);
