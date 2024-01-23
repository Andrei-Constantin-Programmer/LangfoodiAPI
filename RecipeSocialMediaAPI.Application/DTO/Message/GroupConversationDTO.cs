namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record GroupConversationDTO(string ConversationId, string ConnectionId, MessageDTO? LastMessage);
