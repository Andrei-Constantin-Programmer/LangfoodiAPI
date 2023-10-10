using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

public class ConnectionConversation : Conversation
{
    private readonly IConnection _connection;

    public ConnectionConversation(IConnection connection, string conversationId, IEnumerable<Message>? messages = null)
        : base(conversationId, messages)
    {
        _connection = connection;
    }

    public override void SendMessage(Message message)
    {
        if (message.Sender != _connection.Account1 
            && message.Sender != _connection.Account2)
        {
            throw new ArgumentException($"Message {message} cannot be sent to conversation {ConversationId}, as the sender is not part of the conversation.");
        }

        base.SendMessage(message);
    }
}
