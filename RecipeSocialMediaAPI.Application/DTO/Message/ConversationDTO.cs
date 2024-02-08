namespace RecipeSocialMediaAPI.Application.DTO.Message;

public abstract record ConversationDTO(string ConversationId, MessageDTO? LastMessage, int MessagesUnseen = 0);

