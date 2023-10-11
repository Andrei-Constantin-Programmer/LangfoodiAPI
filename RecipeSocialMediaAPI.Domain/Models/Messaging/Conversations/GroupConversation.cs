using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

public class GroupConversation : Conversation
{
    private readonly Group _group;

    public GroupConversation(Group group, string conversationId, IEnumerable<Message>? messages = null)
        : base(conversationId, messages)
    {
        _group = group;
    }

    public override void SendMessage(Message message)
    {
        if (!_group.Users.Any(user => user.Id == message.Sender.Id))
        {
            throw new ArgumentException($"Message {message} cannot be sent to conversation {ConversationId}, as the sender is not part of the conversation.");
        }

        base.SendMessage(message);
    }
}