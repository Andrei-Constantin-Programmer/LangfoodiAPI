using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionQueryRepository
{
    Connection? GetConnection(IUserAccount userAccount1, IUserAccount userAccount2);

    List<Connection> GetConnectionsForUser(IUserAccount userAccount);
}
