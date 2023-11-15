using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationPersistenceRepository
{
    public Conversation CreateConnectionConversation(IConnection connection);
    public Conversation CreateGroupConversation(Group group);
    public bool UpdateConversation(Conversation conversation, IConnection? connection = null, Group? group = null);
}
