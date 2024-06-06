using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class UnsupportedConversationException : Exception
{
    public UnsupportedConversationException(Conversation conversation) : base($"Conversation with id {conversation.ConversationId} found with unsupported type") { }

    [ExcludeFromCodeCoverage(Justification = "Already tested (indirectly) in exception tests for serialization")]
    protected UnsupportedConversationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
