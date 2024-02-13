using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Mappers.Messages;

public class ConversationMapper : IConversationMapper
{
    private readonly IMessageMapper _messageMapper;

    public ConversationMapper(IMessageMapper messageMapper)
    {
        _messageMapper = messageMapper;
    }

    public ConversationDTO MapConversationToConnectionConversationDTO(IUserAccount user, ConnectionConversation conversation)
    {
        var lastMessageDto = GetLastMessage(conversation);
        var unreadCount = GetUnreadCount(user, conversation);

        return new ConversationDTO(
            conversation.ConversationId,
            conversation.Connection.ConnectionId,
            false,
            conversation.Connection.Account1.Id == user.Id ? conversation.Connection.Account2.UserName : conversation.Connection.Account1.UserName,
            conversation.Connection.Account1.Id == user.Id ? conversation.Connection.Account2.ProfileImageId : conversation.Connection.Account1.ProfileImageId,
            lastMessageDto,
            unreadCount);
    }

    public ConversationDTO MapConversationToGroupConversationDTO(IUserAccount user, GroupConversation conversation)
    {
        var lastMessageDto = GetLastMessage(conversation);
        var unreadCount = GetUnreadCount(user, conversation);

        return new ConversationDTO(
            conversation.ConversationId,
            conversation.Group.GroupId,
            true,
            conversation.Group.GroupName,
            null,
            lastMessageDto,
            unreadCount);
    }

    private MessageDTO? GetLastMessage(Conversation conversation)
    {
        var lastMessage = conversation.Messages
            .MaxBy(message => message.SentDate);

        return lastMessage is null
            ? null
            : _messageMapper.MapMessageToMessageDTO(lastMessage);
    }

    private static int GetUnreadCount(IUserAccount user, Conversation conversation) => conversation.Messages
        .Count(message => message.SeenBy.All(u => u.Id != user.Id));
}
