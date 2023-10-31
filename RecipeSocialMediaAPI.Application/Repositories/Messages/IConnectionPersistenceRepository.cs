using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionPersistenceRepository
{
    public Connection CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus);
    bool UpdateConnection(Connection connection);
    bool DeleteConnection(Connection connection);
    bool DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2);
}
