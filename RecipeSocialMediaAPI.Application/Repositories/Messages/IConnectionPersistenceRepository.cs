using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionPersistenceRepository
{
    Task<IConnection> CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus, CancellationToken cancellationToken = default);
    Task<bool> UpdateConnection(IConnection connection, CancellationToken cancellationToken = default);
    Task<bool> DeleteConnection(IConnection connection, CancellationToken cancellationToken = default);
    Task<bool> DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2, CancellationToken cancellationToken = default);
}
