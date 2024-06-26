﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Repositories.Messages;

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
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionById_WhenConnectionIsFound_ReturnMappedConnection()
    {
        // Given
        string connectionId = "conn1";

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

        Expression<Func<ConnectionDocument, bool>> expectedExpression = doc => doc.Id == connectionId;

        ConnectionDocument testDocument = new(testAccount1.Id, testAccount2.Id, "Pending");
        _connectionCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);

        Connection testConnection = new("0", testAccount1, testAccount2, ConnectionStatus.Pending);
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocumentAsync(testDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testConnection);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionAsync(connectionId);

        // Then
        result.Should().Be(testConnection);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionById_WhenConnectionIsNotFound_ReturnNullAndDontMap()
    {
        // Given
        string connectionId = "conn1";

        Expression<Func<ConnectionDocument, bool>> expectedExpression = doc => doc.Id == connectionId;

        _connectionCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConnectionDocument?)null);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionAsync(connectionId);

        // Then
        result.Should().BeNull();
        _connectionDocumentToModelMapperMock
            .Verify(mapper => mapper.MapConnectionFromDocumentAsync(It.IsAny<ConnectionDocument>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionById_WhenMongoThrowsAnException_LogExceptionAndReturnNull()
    {
        // Given
        string connectionId = "conn1";

        Exception testException = new("Test Exception");
        _connectionCollectionMock
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionAsync(connectionId);

        // Then
        result.Should().BeNull();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionById_WhenMapperThrowsException_ThrowException()
    {
        // Given
        string connectionId = "conn1";

        Expression<Func<ConnectionDocument, bool>> expectedExpression = doc => doc.Id == connectionId;

        ConnectionDocument testDocument = new("User1", "User2", "Pending");
        _connectionCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);

        Exception testException = new("Test Exception");
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocumentAsync(It.IsAny<ConnectionDocument>(), It.IsAny<CancellationToken>()))
            .Throws(testException);

        // When
        var testAction = async () => await _connectionQueryRepositorySUT.GetConnectionAsync(connectionId);

        // Then
        await testAction.Should().ThrowAsync<Exception>().WithMessage(testException.Message);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnection_WhenConnectionIsFound_ReturnMappedConnection()
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
        Expression<Func<ConnectionDocument, bool>> expectedExpression = 
            doc => (doc.AccountId1 == testAccount1.Id && doc.AccountId2 == testAccount2.Id)
                || (doc.AccountId1 == testAccount2.Id && doc.AccountId2 == testAccount1.Id);

        ConnectionDocument testDocument = new(testAccount1.Id, testAccount2.Id, "Pending");
        _connectionCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);

        Connection testConnection = new("0", testAccount1, testAccount2, ConnectionStatus.Pending);
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocumentAsync(testDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testConnection);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionAsync(testAccount1, testAccount2);

        // Then
        result.Should().Be(testConnection);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnection_WhenConnectionIsNotFound_ReturnNullAndDontMap()
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

        Expression<Func<ConnectionDocument, bool>> expectedExpression =
            doc => (doc.AccountId1 == testAccount1.Id && doc.AccountId2 == testAccount2.Id)
                || (doc.AccountId1 == testAccount2.Id && doc.AccountId2 == testAccount1.Id);

        _connectionCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConnectionDocument?)null);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionAsync(testAccount1, testAccount2);

        // Then
        result.Should().BeNull();
        _connectionDocumentToModelMapperMock
            .Verify(mapper => mapper.MapConnectionFromDocumentAsync(It.IsAny<ConnectionDocument>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnection_WhenMongoThrowsAnException_LogExceptionAndReturnNull()
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
            .Setup(collection => collection.GetOneAsync(It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionAsync(testAccount1, testAccount2);

        // Then
        result.Should().BeNull();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnection_WhenMapperThrowsException_ThrowException()
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
        Expression<Func<ConnectionDocument, bool>> expectedExpression =
            doc => (doc.AccountId1 == testAccount1.Id && doc.AccountId2 == testAccount2.Id)
                || (doc.AccountId1 == testAccount2.Id && doc.AccountId2 == testAccount1.Id);

        ConnectionDocument testDocument = new(testAccount1.Id, testAccount2.Id, "Pending");
        _connectionCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocument);

        Exception testException = new("Test Exception");
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocumentAsync(It.IsAny<ConnectionDocument>(), It.IsAny<CancellationToken>()))
            .Throws(testException);

        // When
        var testAction = async () => await _connectionQueryRepositorySUT.GetConnectionAsync(testAccount1, testAccount2);

        // Then
        await testAction.Should().ThrowAsync<Exception>().WithMessage(testException.Message);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionsForUser_WhenConnectionsExist_ReturnMappedConnections()
    {
        // Given
        TestUserAccount testAccount = new()
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
            AccountCreationDate = new(2023, 3, 3, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount testAccount3 = new()
        {
            Id = "User3",
            Handler = "user3",
            UserName = "User 3 Name",
            AccountCreationDate = new(2023, 5, 19, 0, 0, 0, TimeSpan.Zero)
        };

        Expression<Func<ConnectionDocument, bool>> expectedExpression = x => x.AccountId1 == testAccount.Id
                                                                             || x.AccountId2 == testAccount.Id;

        List<ConnectionDocument> testDocuments = new()
        {
            new(testAccount.Id, testAccount2.Id, "Pending"),
            new(testAccount3.Id, testAccount.Id, "Favourite")
        };

        List<Connection> testConnections = new()
        {
            new("0", testAccount, testAccount2, ConnectionStatus.Pending),
            new("1", testAccount3, testAccount, ConnectionStatus.Favourite)
        };

        _connectionCollectionMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDocuments);

        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocumentAsync(testDocuments[0], It.IsAny<CancellationToken>()))
            .ReturnsAsync(testConnections[0]);
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocumentAsync(testDocuments[1], It.IsAny<CancellationToken>()))
            .ReturnsAsync(testConnections[1]);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionsForUserAsync(testAccount);

        // Then
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(testConnections[0]);
        result.Should().Contain(testConnections[1]);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionsForUser_WhenNoConnectionsForUserExist_ReturnEmptyList()
    {
        // Given
        TestUserAccount testAccount = new()
        {
            Id = "User1",
            Handler = "user1",
            UserName = "User 1 Name",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        _connectionCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConnectionDocument>());

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionsForUserAsync(testAccount);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionsForUser_WhenMapperThrowsException_RemoveConnectionFromCollectionAndLog()
    {
        // Given
        TestUserAccount testAccount = new()
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
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        Expression<Func<ConnectionDocument, bool>> expectedExpression = x 
            => x.AccountId1 == testAccount.Id
            || x.AccountId2 == testAccount.Id;

        ConnectionDocument testDocument = new(testAccount.Id, testAccount2.Id, "Pending");

        _connectionCollectionMock
            .Setup(collection => collection.GetAllAsync(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConnectionDocument>() { testDocument });

        Exception testException = new("Test Exception");
        _connectionDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConnectionFromDocumentAsync(It.IsAny<ConnectionDocument>(), It.IsAny<CancellationToken>()))
            .Throws(testException);

        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionsForUserAsync(testAccount);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConnectionsForUser_WhenMongoThrowsAnException_LogExceptionAndReturnEmptyList()
    {
        // Given
        TestUserAccount testAccount = new()
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
            AccountCreationDate = new(2023, 3, 3, 0, 0, 0, TimeSpan.Zero)
        };

        Exception testException = new("Test Exception");
        _connectionCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);
    
        // When
        var result = await _connectionQueryRepositorySUT.GetConnectionsForUserAsync(testAccount);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                testException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
