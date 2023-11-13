using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationPersistenceRepository
{
    public Conversation CreateConnectionConversation(Connection connection);
    public Conversation CreateGroupConversation(Group group);
    public Conversation UpdateConversation(Conversation conversation);
    public bool DeleteConversation(Conversation conversation);
    public bool DeleteConversation(string conversationId);
}
