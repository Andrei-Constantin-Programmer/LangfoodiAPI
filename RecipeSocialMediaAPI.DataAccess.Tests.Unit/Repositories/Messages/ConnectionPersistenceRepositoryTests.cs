using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class ConnectionPersistenceRepositoryTests
{
    private readonly Mock<ILogger<ConnectionPersistenceRepository>> _loggerMock;
    private readonly Mock<IConnectionDocumentToModelMapper> _connectionDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<ConnectionDocument>> _connectionCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly ConnectionPersistenceRepository _connectionPersistenceRepositorySUT;

    public ConnectionPersistenceRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ConnectionPersistenceRepository>>();
        _connectionDocumentToModelMapperMock = new Mock<IConnectionDocumentToModelMapper>();
        _connectionCollectionMock = new Mock<IMongoCollectionWrapper<ConnectionDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<ConnectionDocument>())
            .Returns(_connectionCollectionMock.Object);

        _connectionPersistenceRepositorySUT = new ConnectionPersistenceRepository(_loggerMock.Object, _connectionDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

}
