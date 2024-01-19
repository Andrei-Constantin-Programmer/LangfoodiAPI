using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionQueryRepository
{
    IConnection? GetConnection(string connectionId);
    IConnection? GetConnection(IUserAccount userAccount1, IUserAccount userAccount2);

    List<IConnection> GetConnectionsForUser(IUserAccount userAccount);
}
