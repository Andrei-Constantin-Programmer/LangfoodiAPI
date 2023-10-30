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

public class ConnectionQueryRepositoryTests
{
    private readonly Mock<ILogger<ConnectionQueryRepository>> _loggerMock;
    private readonly Mock<IConnectionDocumentToModelMapper> _connectionDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<ConnectionDocument>> _connectionCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly ConnectionQueryRepository _connectionQueryRepositorySUT;

    public ConnectionQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ConnectionQueryRepository>>();
        _connectionDocumentToModelMapperMock = new Mock<IConnectionDocumentToModelMapper>();
        _connectionCollectionMock = new Mock<IMongoCollectionWrapper<ConnectionDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<ConnectionDocument>())
            .Returns(_connectionCollectionMock.Object);

        _connectionQueryRepositorySUT = new(_loggerMock.Object, _connectionDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConnection_WhenConnectionIsFound_ReturnMappedConnection()
    {
        // Given
        TestUserAccount testAccount1 = new()
        {
            Id = "User1",
            Handler = "user1",
            UserName = "User 1 Name",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount testAccount2 = new()
        {
            Id = "User2",
            Handler = "user2",
            UserName = "User 2 Name",
            AccountCreationDate = new(2023, 2, 5, 0, 0, 0, TimeSpan.Zero)
        };
        Expression<Func<ConnectionDocument, bool>> expectedExpression = x => x.AccountId1 == testAccount1.Id
                                                                             && x.AccountId2 == testAccount2.Id;

        ConnectionDocument testDocument = new()
        {
            AccountId1 = testAccount1.Id,
            AccountId2 = testAccount2.Id,
            ConnectionStatus = "Pending"
        };
        _connectionCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(testDocument);

        Connection testConnection = new(testAccount1, testAccount2, ConnectionStatus.Pending);
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocument(testDocument))
            .Returns(testConnection);

        // When
        var result = _connectionQueryRepositorySUT.GetConnection(testAccount1, testAccount2);

        // Then
        result.Should().Be(testConnection);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConnection_WhenConnectionIsNotFound_ReturnNullAndDontMap()
    {
        // Given
        TestUserAccount testAccount1 = new()
        {
            Id = "User1",
            Handler = "user1",
            UserName = "User 1 Name",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount testAccount2 = new()
        {
            Id = "User2",
            Handler = "user2",
            UserName = "User 2 Name",
            AccountCreationDate = new(2023, 2, 5, 0, 0, 0, TimeSpan.Zero)
        };

        Expression<Func<ConnectionDocument, bool>> expectedExpression = x => x.AccountId1 == testAccount1.Id
                                                                             && x.AccountId2 == testAccount2.Id;

        _connectionCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns((ConnectionDocument?)null);

        // When
        var result = _connectionQueryRepositorySUT.GetConnection(testAccount1, testAccount2);

        // Then
        result.Should().BeNull();
        _connectionDocumentToModelMapperMock
            .Verify(mapper => mapper.MapConnectionFromDocument(It.IsAny<ConnectionDocument>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConnection_WhenMongoThrowsAnException_LogExceptionAndReturnNull()
    {
        // Given
        TestUserAccount testAccount1 = new()
        {
            Id = "User1",
            Handler = "user1",
            UserName = "User 1 Name",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount testAccount2 = new()
        {
            Id = "User2",
            Handler = "user2",
            UserName = "User 2 Name",
            AccountCreationDate = new(2023, 2, 5, 0, 0, 0, TimeSpan.Zero)
        };

        Exception testException = new("Test Exception");
        _connectionCollectionMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<ConnectionDocument, bool>>>()))
            .Throws(testException);

        // When
        var result = _connectionQueryRepositorySUT.GetConnection(testAccount1, testAccount2);

        // Then
        result.Should().BeNull();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
