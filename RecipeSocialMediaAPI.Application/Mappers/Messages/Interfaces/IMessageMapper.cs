using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;

public interface IMessageMapper
{
    MessageDTO MapMessageToMessageDTO(Message message);
    MessageDetailedDTO MapMessageToDetailedMessageDTO(Message message);
}