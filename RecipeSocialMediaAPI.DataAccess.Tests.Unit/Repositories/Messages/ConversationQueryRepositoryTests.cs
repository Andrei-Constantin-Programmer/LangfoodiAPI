using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Connections;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class ConversationQueryRepositoryTests
{
    private readonly Mock<ILogger<ConversationQueryRepository>> _loggerMock;
    private readonly Mock<IConversationDocumentToModelMapper> _conversationDocumentToModelMapperMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IMongoCollectionWrapper<ConversationDocument>> _conversationCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly ConversationQueryRepository _conversationQueryRepositorySUT;

    public ConversationQueryRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<ConversationQueryRepository>>();
        _conversationDocumentToModelMapperMock = new Mock<IConversationDocumentToModelMapper>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _conversationCollectionMock = new Mock<IMongoCollectionWrapper<ConversationDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<ConversationDocument>())
            .Returns(_conversationCollectionMock.Object);

        _conversationQueryRepositorySUT = new(
            _loggerMock.Object,
            _conversationDocumentToModelMapperMock.Object,
            _mongoCollectionFactoryMock.Object,
            _connectionQueryRepositoryMock.Object,
            _groupQueryRepositoryMock.Object,
            _messageQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationById_WhenConnectionConversationExists_ReturnMappedConversation()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2",
        };

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 15, 30, TimeSpan.Zero), null)
        };

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[0].Id))
            .Returns(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[1].Id))
            .Returns(messages[1]);

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(connection.ConnectionId))
            .Returns(connection);

        ConnectionConversation conversation = new(connection, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), connection.ConnectionId, null, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversation.ConversationId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, connection, null, messages))
            .Returns(conversation);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationById(conversation.ConversationId) as ConnectionConversation;

        // Then
        result.Should().Be(conversation);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationById_WhenGroupConversationExists_ReturnMappedConversation()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2",
        };

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 15, 30, TimeSpan.Zero), null)
        };

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[0].Id))
            .Returns(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[1].Id))
            .Returns(messages[1]);

        Group group = new("g1", "Group", "Group Desc", new List<IUserAccount>() { user1, user2 });
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(group.GroupId))
            .Returns(group);

        GroupConversation conversation = new(group, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), null, group.GroupId, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversation.ConversationId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, null, group, messages))
            .Returns(conversation);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationById(conversation.ConversationId) as GroupConversation;

        // Then
        result.Should().Be(conversation);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationById_WhenConversationDoesNotExist_ReturnNull()
    {
        // Given
        string conversationId = "convo1";

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversationId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns((ConversationDocument?)null);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationById(conversationId);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationById_WhenMongoThrowsAnException_ReturnNullAndLogException()
    {
        // Given
        string conversationId = "convo1";

        Exception testException = new("Test Exception");

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversationId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Throws(testException);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationById(conversationId);

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
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationByConnection_WhenConversationExists_ReturnMappedConversation()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2",
        };

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 15, 30, TimeSpan.Zero), null)
        };

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[0].Id))
            .Returns(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[1].Id))
            .Returns(messages[1]);

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(connection.ConnectionId))
            .Returns(connection);

        ConnectionConversation conversation = new(connection, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), connection.ConnectionId, null, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.ConnectionId == connection.ConnectionId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, connection, null, messages))
            .Returns(conversation);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationByConnection(connection.ConnectionId) as ConnectionConversation;

        // Then
        result.Should().Be(conversation);
        _groupQueryRepositoryMock
            .Verify(repo => repo.GetGroupById(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationByConnection_WhenConversationDoesNotExist_ReturnNull()
    {
        // Given
        string connectionId = "conn1";

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.ConnectionId == connectionId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns((ConversationDocument?)null);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationByConnection(connectionId);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationByConnection_WhenMongoThrowsAnException_ReturnNullAndLogException()
    {
        // Given
        string connectionId = "conn1";

        Exception testException = new("Test Exception");

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.ConnectionId == connectionId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Throws(testException);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationByConnection(connectionId);

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
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationByGroup_WhenConversationExists_ReturnMappedConversation()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2",
        };

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 15, 30, TimeSpan.Zero), null)
        };

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[0].Id))
            .Returns(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(messages[1].Id))
            .Returns(messages[1]);

        Group group = new("g1", "Group", "Group Desc", new List<IUserAccount>() { user1, user2 });
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupById(group.GroupId))
            .Returns(group);

        GroupConversation conversation = new(group, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), null, group.GroupId, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.GroupId == group.GroupId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, null, group, messages))
            .Returns(conversation);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationByGroup(group.GroupId) as GroupConversation;

        // Then
        result.Should().Be(conversation);
        _connectionQueryRepositoryMock
            .Verify(repo => repo.GetConnection(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationByGroup_WhenConversationDoesNotExist_ReturnNull()
    {
        // Given
        string groupId = "g1";

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.GroupId == groupId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns((ConversationDocument?)null);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationByGroup(groupId);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void GetConversationByGroup_WhenMongoThrowsAnException_ReturnNullAndLogException()
    {
        // Given
        string connectionId = "conn1";

        Exception testException = new("Test Exception");

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.GroupId == connectionId;
        _conversationCollectionMock
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Throws(testException);

        // When
        var result = _conversationQueryRepositorySUT.GetConversationByGroup(connectionId);

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
}
