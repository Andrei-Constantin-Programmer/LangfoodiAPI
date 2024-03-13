using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConversationPersistenceRepository
{
    Task<Conversation> CreateConnectionConversationAsync(IConnection connection, CancellationToken cancellationToken = default);
    Task<Conversation> CreateGroupConversationAsync(Group group, CancellationToken cancellationToken = default);
    Task<bool> UpdateConversationAsync(Conversation conversation, IConnection? connection = null, Group? group = null, CancellationToken cancellationToken = default);
}
