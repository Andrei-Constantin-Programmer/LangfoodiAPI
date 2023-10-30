using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConnectionPersistenceRepository : IConnectionPersistenceRepository
{
    private readonly ILogger<ConnectionPersistenceRepository> _logger;
    private readonly IConnectionDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConnectionDocument> _connectionCollection;

    public ConnectionPersistenceRepository(ILogger<ConnectionPersistenceRepository> logger, IConnectionDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = mapper;
        _connectionCollection = mongoCollectionFactory.CreateCollection<ConnectionDocument>();
    }

    public Connection CreateConnection(IUserAccount userAccount1, IUserAccount userAccount2, ConnectionStatus connectionStatus) => throw new NotImplementedException();
    public bool UpdateConnection(Connection connection) => throw new NotImplementedException();
    public bool DeleteConnection(Connection connection) => throw new NotImplementedException();
    public bool DeleteConnection(IUserAccount userAccount1, IUserAccount userAccount2) => throw new NotImplementedException();
}
