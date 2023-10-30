using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConnection_WhenUpdateIsSuccessful_ReturnTrue()
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

        Connection testConnection = new(testUser1, testUser2, ConnectionStatus.Connected);

        Expression<Func<ConnectionDocument, bool>> expectedExpression = 
            doc => (doc.AccountId1 == testUser1.Id && doc.AccountId2 == testUser2.Id)
                || (doc.AccountId1 == testUser2.Id && doc.AccountId2 == testUser1.Id);

        _connectionCollectionMock
            .Setup(collection => collection.UpdateRecord(It.Is<ConnectionDocument>(document 
                => document.AccountId1 == testUser1.Id
                   && document.AccountId2 == testUser2.Id
                   && document.ConnectionStatus == "Connected"),
                   It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(true);

        // When
        var result = _connectionPersistenceRepositorySUT.UpdateConnection(testConnection);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConnection_WhenUpdateIsUnsuccessful_ReturnFalse()
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

        Connection testConnection = new(testUser1, testUser2, ConnectionStatus.Connected);

        Expression<Func<ConnectionDocument, bool>> expectedExpression =
            doc => (doc.AccountId1 == testUser1.Id && doc.AccountId2 == testUser2.Id)
                || (doc.AccountId1 == testUser2.Id && doc.AccountId2 == testUser1.Id);

        _connectionCollectionMock
            .Setup(collection => collection.UpdateRecord(It.Is<ConnectionDocument>(document
                => document.AccountId1 == testUser1.Id
                   && document.AccountId2 == testUser2.Id
                   && document.ConnectionStatus == "Connected"),
                   It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(false);

        // When
        var result = _connectionPersistenceRepositorySUT.UpdateConnection(testConnection);

        // Then
        result.Should().BeFalse();
    }
}
