using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionPersistenceRepository
{
    Task<IConnection> CreateConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus, CancellationToken cancellationToken = default);
    Task<bool> UpdateConnectionAsync(IConnection connection, CancellationToken cancellationToken = default);
    Task<bool> DeleteConnectionAsync(IConnection connection, CancellationToken cancellationToken = default);
    Task<bool> DeleteConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, CancellationToken cancellationToken = default);
}
