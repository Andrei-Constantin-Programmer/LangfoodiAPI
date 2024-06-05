namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record ConversationDto(
    string Id,
    string ConnectionOrGroupId,
    bool IsGroup,
    string Name,
    string? ThumbnailId,
    MessageDto? LastMessage,
    List<string> UserIds,
    int MessagesUnseen = 0
);
