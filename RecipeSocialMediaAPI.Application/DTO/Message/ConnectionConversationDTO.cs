namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record ConnectionConversationDTO(string ConversationId, string ConnectionId, MessageDTO? LastMessage, int MessagesUnseen = 0):
    ConversationDTO(ConversationId, LastMessage, MessagesUnseen);

