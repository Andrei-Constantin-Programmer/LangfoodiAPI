namespace RecipeSocialMediaAPI.Application.DTO.Message;

public record ConversationDTO(string ConversationId, MessageDTO? LastMessage, int MessagesUnseen = 0);

