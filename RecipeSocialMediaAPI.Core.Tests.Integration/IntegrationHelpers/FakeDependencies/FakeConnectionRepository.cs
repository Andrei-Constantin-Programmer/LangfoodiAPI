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

    public IConnection? GetConnection(string connectionId) => _collection.FirstOrDefault(conn => conn.ConnectionId == connectionId);

    public IConnection? GetConnection(IUserAccount userAccount1, IUserAccount userAccount2) => 
        _collection.FirstOrDefault(conn => (conn.Account1 == userAccount1 && conn.Account2 == userAccount2)
                                              || (conn.Account1 == userAccount2 && conn.Account2 == userAccount1));

    public List<IConnection> GetConnectionsForUser(IUserAccount userAccount) => 
        _collection
            .Where(conn => conn.Account1 == userAccount || conn.Account2 == userAccount)
            .ToList();

    public IConnection CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus)
    {
        var id = _collection.Count.ToString();
        Connection newConnection = new(id, userAccount1, userAccount2, connectionStatus);
        _collection.Add(newConnection);

        return newConnection;
    }

    public bool UpdateConnection(IConnection connection)
    {
        IConnection? existingConnection = _collection.FirstOrDefault(x => x.ConnectionId == connection.ConnectionId);
        if (existingConnection is null)
        {
            return false;
        }

        Connection updatedConnection = new(connection.ConnectionId, connection.Account1, connection.Account2, connection.Status);

        _collection.Remove(existingConnection);
        _collection.Add(updatedConnection);

        return true;
    }

    public bool DeleteConnection(IConnection connection) => _collection.Remove(connection);

    public bool DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2)
    {
        var connection = GetConnection(userAccount1, userAccount2);

        return connection is not null && _collection.Remove(connection);
    }
}
