using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationPersistenceRepository
{
    Task<Conversation> CreateConnectionConversation(IConnection connection, CancellationToken cancellationToken = default);
    Task<Conversation> CreateGroupConversationAsync(Group group, CancellationToken cancellationToken = default);
    Task<bool> UpdateConversation(Conversation conversation, IConnection? connection = null, Group? group = null, CancellationToken cancellationToken = default);
}
