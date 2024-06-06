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

    public ConversationDto MapConversationToConnectionConversationDTO(IUserAccount user, ConnectionConversation conversation)
    {
        var lastMessageDto = GetLastMessage(conversation);
        var unreadCount = GetUnreadCount(user, conversation);

        return new ConversationDto(
            conversation.ConversationId,
            conversation.Connection.ConnectionId,
            false,
            conversation.Connection.Account1.Id == user.Id ? conversation.Connection.Account2.UserName : conversation.Connection.Account1.UserName,
            conversation.Connection.Account1.Id == user.Id ? conversation.Connection.Account2.ProfileImageId : conversation.Connection.Account1.ProfileImageId,
            lastMessageDto,
            new() { conversation.Connection.Account1.Id, conversation.Connection.Account2.Id },
            unreadCount);
    }

    public ConversationDto MapConversationToGroupConversationDTO(IUserAccount user, GroupConversation conversation)
    {
        var lastMessageDto = GetLastMessage(conversation);
        var unreadCount = GetUnreadCount(user, conversation);

        return new ConversationDto(
            conversation.ConversationId,
            conversation.Group.GroupId,
            true,
            conversation.Group.GroupName,
            null,
            lastMessageDto,
            conversation.Group.Users.Select(user => user.Id).ToList(),
            unreadCount);
    }

    private MessageDto? GetLastMessage(Conversation conversation)
    {
        var lastMessage = conversation.GetMessages()
            .MaxBy(message => message.SentDate);

        return lastMessage is null
            ? null
            : _messageMapper.MapMessageToMessageDTO(lastMessage);
    }

    private static int GetUnreadCount(IUserAccount user, Conversation conversation) => conversation.GetMessages()
        .Count(message => message.GetSeenBy().TrueForAll(u => u.Id != user.Id));
}
