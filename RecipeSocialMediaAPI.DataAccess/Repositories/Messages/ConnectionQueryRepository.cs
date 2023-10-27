using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConnectionQueryRepository : IConnectionQueryRepository
{
    public Connection? GetConnection(IUserAccount userAccount1, IUserAccount userAccount2)
    {
        throw new NotImplementedException();
    }

    public List<Connection> GetConnectionsForUser(IUserAccount userAccount)
    {
        throw new NotImplementedException();
    }
}
