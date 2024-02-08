namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record GroupConversationDTO(string ConversationId, string GroupId, MessageDTO? LastMessage, int MessagesUnseen = 0) :
    ConversationDTO(ConversationId, LastMessage, MessagesUnseen);

