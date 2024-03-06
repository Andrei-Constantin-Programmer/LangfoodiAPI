using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Messages;

public interface IConnectionQueryRepository
{
    Task<IConnection?> GetConnectionAsync(string connectionId, CancellationToken cancellationToken = default);
    Task<IConnection?> GetConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, CancellationToken cancellationToken = default);

    Task<List<IConnection>> GetConnectionsForUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default);
}
