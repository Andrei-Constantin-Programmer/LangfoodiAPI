using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateConnection_WhenConnectionIsValid_AddConnectionToCollectionAndReturnMappedConnection()
    {
        // Given
        TestUserAccount testUser1 = new()
        {
            Id = "User1",
            Handler = "user1",
            UserName = "Username1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        TestUserAccount testUser2 = new()
        {
            Id = "User2",
            Handler = "user2",
            UserName = "Username2",
            AccountCreationDate = new(2023, 3, 3, 0, 0, 0, TimeSpan.Zero)
        };

        ConnectionDocument testDocument = new()
        {
            AccountId1 = testUser1.Id,
            AccountId2 = testUser2.Id,
            ConnectionStatus = "Pending"
        };
        Connection testConnection = new(testUser1, testUser2, ConnectionStatus.Pending);

        _connectionCollectionMock
            .Setup(collection => collection.Insert(It.IsAny<ConnectionDocument>()))
            .Returns(testDocument);
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocument(testDocument))
            .Returns(testConnection);

        // When
        var result = _connectionPersistenceRepositorySUT.CreateConnection(testUser1, testUser2, testConnection.Status);

        // Then
        result.Should().Be(testConnection);
        _connectionCollectionMock
            .Verify(collection => collection.Insert(It.Is<ConnectionDocument>(document
                => document.AccountId1 == testUser1.Id
                   && document.AccountId2 == testUser2.Id
                   && document.ConnectionStatus == "Pending")), 
                Times.Once);
    }
}
