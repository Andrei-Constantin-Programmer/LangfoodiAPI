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

    public ConnectionConversationDTO MapConversationToConnectionConversationDTO(IUserAccount user, ConnectionConversation conversation)
    {
        var lastMessageDto = GetLastMessage(conversation);
        var unreadCount = GetUnreadCount(user, conversation);

        return new ConnectionConversationDTO(conversation.ConversationId, conversation.Connection.ConnectionId, lastMessageDto, unreadCount);
    }

    public GroupConversationDTO MapConversationToGroupConversationDTO(IUserAccount user, GroupConversation conversation)
    {
        var lastMessageDto = GetLastMessage(conversation);
        var unreadCount = GetUnreadCount(user, conversation);

        return new GroupConversationDTO(conversation.ConversationId, conversation.Group.GroupId, lastMessageDto, unreadCount);
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
