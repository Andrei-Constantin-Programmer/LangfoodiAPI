using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.TestHelpers;

internal class TestConversation : Conversation
{
    public TestConversation(string conversationId, IEnumerable<Message>? messages = null) : base(conversationId, messages)
    {
    }
}

