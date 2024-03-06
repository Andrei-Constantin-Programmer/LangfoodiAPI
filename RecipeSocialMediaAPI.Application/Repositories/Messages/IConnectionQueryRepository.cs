using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionQueryRepository
{
    Task<IConnection?> GetConnection(string connectionId, CancellationToken cancellationToken = default);
    Task<IConnection?> GetConnection(IUserAccount userAccount1, IUserAccount userAccount2, CancellationToken cancellationToken = default);

    Task<List<IConnection>> GetConnectionsForUser(IUserAccount userAccount, CancellationToken cancellationToken = default);
}
