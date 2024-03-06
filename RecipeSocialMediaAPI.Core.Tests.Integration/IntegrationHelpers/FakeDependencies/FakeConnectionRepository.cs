using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeConnectionRepository : IConnectionQueryRepository, IConnectionPersistenceRepository
{
    private readonly List<IConnection> _collection;

    public FakeConnectionRepository()
    {
        _collection = new();
    }

    public async Task<IConnection?> GetConnectionAsync(string connectionId, CancellationToken cancellationToken = default) => await Task.FromResult(_collection.FirstOrDefault(conn => conn.ConnectionId == connectionId));

    public async Task<IConnection?> GetConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, CancellationToken cancellationToken = default) => 
        await Task.FromResult(_collection.FirstOrDefault(conn => (conn.Account1.Id == userAccount1.Id && conn.Account2.Id == userAccount2.Id)
                                              || (conn.Account1.Id == userAccount2.Id && conn.Account2.Id == userAccount1.Id)));

    public Task<List<IConnection>> GetConnectionsForUserAsync(IUserAccount userAccount, CancellationToken cancellationToken = default) =>
        Task.FromResult(_collection
            .Where(conn => conn.Account1.Id == userAccount.Id || conn.Account2.Id == userAccount.Id)
            .ToList());

    public async Task<IConnection> CreateConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus, CancellationToken cancellationToken = default)
    {
        var id = _collection.Count.ToString();
        Connection newConnection = new(id, userAccount1, userAccount2, connectionStatus);
        _collection.Add(newConnection);

        return await Task.FromResult(newConnection);
    }

    public async Task<bool> UpdateConnectionAsync(IConnection connection, CancellationToken cancellationToken = default)
    {
        IConnection? existingConnection = _collection.FirstOrDefault(x => x.ConnectionId == connection.ConnectionId);
        if (existingConnection is null)
        {
            return false;
        }

        Connection updatedConnection = new(connection.ConnectionId, connection.Account1, connection.Account2, connection.Status);

        _collection.Remove(existingConnection);
        _collection.Add(updatedConnection);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteConnectionAsync(IConnection connection, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Remove(connection));

    public async Task<bool> DeleteConnectionAsync(IUserAccount userAccount1, IUserAccount userAccount2, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(userAccount1, userAccount2, cancellationToken);

        return connection is not null && _collection.Remove(connection);
    }
}
