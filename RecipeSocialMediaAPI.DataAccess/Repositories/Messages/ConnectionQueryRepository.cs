using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

public class ConnectionQueryRepository : IConnectionQueryRepository
{
    private readonly ILogger<ConnectionQueryRepository> _logger;
    private readonly IConnectionDocumentToModelMapper _mapper;
    private readonly IMongoCollectionWrapper<ConnectionDocument> _connectionCollection;

    public ConnectionQueryRepository(ILogger<ConnectionQueryRepository> logger, IConnectionDocumentToModelMapper connectionDocumentToModelMapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = connectionDocumentToModelMapper;
        _connectionCollection = mongoCollectionFactory.CreateCollection<ConnectionDocument>();
    }

    public Connection? GetConnection(IUserAccount userAccount1, IUserAccount userAccount2)
    {
        var connectionDocument = _connectionCollection.Find(
            connectionDoc => connectionDoc.AccountId1 == userAccount1.Id 
                             && connectionDoc.AccountId2 == userAccount2.Id);

        return _mapper.MapConnectionFromDocument(connectionDocument!);
    }

    public List<Connection> GetConnectionsForUser(IUserAccount userAccount)
    {
        throw new NotImplementedException();
    }
}
