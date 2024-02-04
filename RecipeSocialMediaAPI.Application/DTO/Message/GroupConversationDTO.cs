namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record GroupConversationDTO(string ConversationId, string GroupId, MessageDTO? LastMessage);
