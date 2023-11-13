using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConversationPersistenceRepository : IConversationPersistenceRepository
{
    public Conversation CreateConnectionConversation(Connection connection)
    {
        throw new NotImplementedException();
    }

    public Conversation CreateGroupConversation(Group group)
    {
        throw new NotImplementedException();
    }

    public Conversation UpdateConversation(Conversation conversation)
    {
        throw new NotImplementedException();
    }

    public bool DeleteConversation(Conversation conversation)
    {
        throw new NotImplementedException();
    }

    public bool DeleteConversation(string conversationId)
    {
        throw new NotImplementedException();
    }
}
