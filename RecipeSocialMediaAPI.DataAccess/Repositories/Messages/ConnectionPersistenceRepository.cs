using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConnectionPersistenceRepository : IConnectionPersistenceRepository
{
    public Connection CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus) => throw new NotImplementedException();
    public bool UpdateConnection(Connection connection) => throw new NotImplementedException();
    public bool DeleteConnection(Connection connection) => throw new NotImplementedException();
    public bool DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2) => throw new NotImplementedException();
}
