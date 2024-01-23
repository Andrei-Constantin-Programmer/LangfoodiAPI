namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record ConnectionConversationDTO(string ConversationId, string ConnectionId, MessageDTO LastMessage);
