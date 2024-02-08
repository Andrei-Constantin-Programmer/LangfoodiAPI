using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UnsupportedConversationException : Exception
{
    public UnsupportedConversationException(Conversation conversation) : base($"Conversation with id {conversation.ConversationId} found with unsupported type") { }
}
