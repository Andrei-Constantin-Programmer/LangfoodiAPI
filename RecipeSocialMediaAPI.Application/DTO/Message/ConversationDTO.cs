namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record ConversationDTO(
    string ConversationId,
    string ConnectionOrGroupId,
    bool IsGroup,
    string ConversationName,
    string? ThumbnailId,
    MessageDTO? LastMessage,
    int MessagesUnseen = 0
);
