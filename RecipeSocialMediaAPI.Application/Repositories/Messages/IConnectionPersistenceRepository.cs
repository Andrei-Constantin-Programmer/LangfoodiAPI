using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionPersistenceRepository
{
    public IConnection CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus);
    bool UpdateConnection(IConnection connection);
    bool DeleteConnection(IConnection connection);
    bool DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2);
}
