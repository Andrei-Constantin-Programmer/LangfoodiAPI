using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionPersistenceRepository
{
    Task<IConnection> CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus, CancellationToken cancellationToken = default);
    bool UpdateConnection(IConnection connection);
    bool DeleteConnection(IConnection connection);
    bool DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2);
}
