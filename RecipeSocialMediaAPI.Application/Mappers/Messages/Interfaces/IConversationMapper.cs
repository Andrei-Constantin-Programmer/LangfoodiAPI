using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;

public interface IConversationMapper
{
    ConversationDTO MapConversationToConnectionConversationDTO(IUserAccount user, ConnectionConversation conversation);
    ConversationDTO MapConversationToGroupConversationDTO(IUserAccount user, GroupConversation conversation);
}