using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Repositories.Messages;

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
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationById_WhenConnectionConversationExists_ReturnMappedConversation()
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
            .Setup(repo => repo.GetMessageAsync(messages[0].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(messages[1].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[1]);

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection);

        ConnectionConversation conversation = new(connection, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), connection.ConnectionId, null, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversation.ConversationId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, connection, null, messages))
            .Returns(conversation);

        // When
        var result = (await _conversationQueryRepositorySUT.GetConversationByIdAsync(conversation.ConversationId)) as ConnectionConversation;

        // Then
        result.Should().Be(conversation);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationById_WhenGroupConversationExists_ReturnMappedConversation()
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
            .Setup(repo => repo.GetMessageAsync(messages[0].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(messages[1].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[1]);

        Group group = new("g1", "Group", "Group Desc", new List<IUserAccount>() { user1, user2 });
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(group.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        GroupConversation conversation = new(group, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), null, group.GroupId, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversation.ConversationId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, null, group, messages))
            .Returns(conversation);

        // When
        var result = (await _conversationQueryRepositorySUT.GetConversationByIdAsync(conversation.ConversationId)) as GroupConversation;

        // Then
        result.Should().Be(conversation);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationById_WhenConversationDoesNotExist_ReturnNull()
    {
        // Given
        string conversationId = "convo1";

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversationId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConversationDocument?)null);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationByIdAsync(conversationId);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationById_WhenMongoThrowsAnException_ReturnNullAndLogException()
    {
        // Given
        string conversationId = "convo1";

        Exception testException = new("Test Exception");

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.Id == conversationId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationByIdAsync(conversationId);

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
    public async Task GetConversationByConnection_WhenConversationExists_ReturnMappedConversation()
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
            .Setup(repo => repo.GetMessageAsync(messages[0].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(messages[1].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[1]);

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection);

        ConnectionConversation conversation = new(connection, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), connection.ConnectionId, null, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.ConnectionId == connection.ConnectionId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, connection, null, messages))
            .Returns(conversation);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationByConnectionAsync(connection.ConnectionId);

        // Then
        result.Should().Be(conversation);
        _groupQueryRepositoryMock
            .Verify(repo => repo.GetGroupByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationByConnection_WhenConversationDoesNotExist_ReturnNull()
    {
        // Given
        string connectionId = "conn1";

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.ConnectionId == connectionId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConversationDocument?)null);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationByConnectionAsync(connectionId);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationByConnection_WhenMongoThrowsAnException_ReturnNullAndLogException()
    {
        // Given
        string connectionId = "conn1";

        Exception testException = new("Test Exception");

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.ConnectionId == connectionId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationByConnectionAsync(connectionId);

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
    public async Task GetConversationByGroup_WhenConversationExists_ReturnMappedConversation()
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
            .Setup(repo => repo.GetMessageAsync(messages[0].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[0]);
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(messages[1].Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages[1]);

        Group group = new("g1", "Group", "Group Desc", new List<IUserAccount>() { user1, user2 });
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(group.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        GroupConversation conversation = new(group, "convo1", messages);
        ConversationDocument document = new(messages.Select(message => message.Id).ToList(), null, group.GroupId, conversation.ConversationId);

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.GroupId == group.GroupId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document, null, group, messages))
            .Returns(conversation);

        // When
        var result = (await _conversationQueryRepositorySUT.GetConversationByGroupAsync(group.GroupId));

        // Then
        result.Should().Be(conversation);
        _connectionQueryRepositoryMock
            .Verify(repo => repo.GetConnectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationByGroup_WhenConversationDoesNotExist_ReturnNull()
    {
        // Given
        string groupId = "g1";

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.GroupId == groupId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConversationDocument?)null);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationByGroupAsync(groupId);

        // Then
        result.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationByGroup_WhenMongoThrowsAnException_ReturnNullAndLogException()
    {
        // Given
        string connectionId = "conn1";

        Exception testException = new("Test Exception");

        Expression<Func<ConversationDocument, bool>> expectedExpression = doc => doc.GroupId == connectionId;
        _conversationCollectionMock
            .Setup(collection => collection.GetOneAsync(
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationByGroupAsync(connectionId);

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
    public async Task GetConversationsByUser_WhenConversationsExist_ReturnMappedConversations()
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
        TestUserAccount user3 = new()
        {
            Id = "u3",
            Handler = "user3",
            UserName = "User 3",
        };
        TestUserAccount user4 = new()
        {
            Id = "u4",
            Handler = "user4",
            UserName = "User 4",
        };

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 15, 30, TimeSpan.Zero), null),
            new TestMessage("m3", user3, new(2024, 1, 1, 0, 16, 20, TimeSpan.Zero), null),
            new TestMessage("m4", user4, new(2024, 1, 1, 0, 18, 42, TimeSpan.Zero), null),
            new TestMessage("m5", user3, new(2024, 1, 1, 1, 0, 11, TimeSpan.Zero), null),
            new TestMessage("m6", user2, new(2024, 1, 1, 1, 12, 53, TimeSpan.Zero), null),
        };

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string messageId, CancellationToken _) => messages.First(message => message.Id == messageId));

        Connection connection1 = new("conn1", user1, user2, ConnectionStatus.Connected);
        Connection connection2 = new("conn2", user1, user3, ConnectionStatus.Pending);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection1.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection1);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection2.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection2);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionsForUserAsync(user1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IConnection>() { connection1, connection2 });

        Group group1 = new("g1", "Group 1", "Group Description", new List<IUserAccount>() { user1, user2, user3 });
        Group group2 = new("g2", "Group 2", "Group Description", new List<IUserAccount>() { user1, user3, user4 });
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(group1.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group1);
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(group2.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group2);
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupsByUserAsync(user1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Group>() { group1, group2 });

        ConnectionConversation conversation1 = new(connection1, "convo1", messages.GetRange(0, 2));
        ConnectionConversation conversation2 = new(connection2, "convo2", messages.GetRange(2, 1));
        GroupConversation conversation3 = new(group1, "convo3", messages.GetRange(3, 2));
        GroupConversation conversation4 = new(group2, "convo4", messages.GetRange(4, 1));

        ConversationDocument document1 = new(conversation1.GetMessages().Select(message => message.Id).ToList(), connection1.ConnectionId, null, conversation1.ConversationId);
        ConversationDocument document2 = new(conversation2.GetMessages().Select(message => message.Id).ToList(), connection2.ConnectionId, null, conversation2.ConversationId);
        ConversationDocument document3 = new(conversation3.GetMessages().Select(message => message.Id).ToList(), null, group1.GroupId, conversation3.ConversationId);
        ConversationDocument document4 = new(conversation4.GetMessages().Select(message => message.Id).ToList(), null, group2.GroupId, conversation4.ConversationId);

        _conversationCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<ConversationDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConversationDocument>() { document1, document2, document3, document4 });

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document1, connection1, null, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation1.GetMessages()))))
            .Returns(conversation1);
        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document2, connection2, null, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation2.GetMessages()))))
            .Returns(conversation2);
        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document3, null, group1, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation3.GetMessages()))))
            .Returns(conversation3);
        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document4, null, group2, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation4.GetMessages()))))
            .Returns(conversation4);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationsByUserAsync(user1);

        // Then
        result.Should().BeEquivalentTo(new List<Conversation>() { conversation1, conversation2, conversation3, conversation4 });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationsByUser_WhenConversationMapperThrows_ReturnMappedConversationsIgnoringFailingConversation()
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
        TestUserAccount user3 = new()
        {
            Id = "u3",
            Handler = "user3",
            UserName = "User 3",
        };
        TestUserAccount user4 = new()
        {
            Id = "u4",
            Handler = "user4",
            UserName = "User 4",
        };

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null),
            new TestMessage("m2", user2, new(2024, 1, 1, 0, 15, 30, TimeSpan.Zero), null),
            new TestMessage("m3", user3, new(2024, 1, 1, 0, 16, 20, TimeSpan.Zero), null),
            new TestMessage("m4", user4, new(2024, 1, 1, 0, 18, 42, TimeSpan.Zero), null),
            new TestMessage("m5", user3, new(2024, 1, 1, 1, 0, 11, TimeSpan.Zero), null),
            new TestMessage("m6", user2, new(2024, 1, 1, 1, 12, 53, TimeSpan.Zero), null),
        };

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string messageId, CancellationToken _) => messages.First(message => message.Id == messageId));

        Connection connection1 = new("conn1", user1, user2, ConnectionStatus.Connected);
        Connection connection2 = new("conn2", user1, user3, ConnectionStatus.Pending);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection1.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection1);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection2.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection2);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionsForUserAsync(user1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IConnection>() { connection1, connection2 });

        Group group1 = new("g1", "Group 1", "Group Description", new List<IUserAccount>() { user1, user2, user3 });
        Group group2 = new("g2", "Group 2", "Group Description", new List<IUserAccount>() { user1, user3, user4 });
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(group1.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group1);
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupByIdAsync(group2.GroupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group2);
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupsByUserAsync(user1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Group>() { group1, group2 });

        ConnectionConversation conversation1 = new(connection1, "convo1", messages.GetRange(0, 2));
        ConnectionConversation conversation2 = new(connection2, "convo2", messages.GetRange(2, 1));
        GroupConversation conversation3 = new(group1, "convo3", messages.GetRange(3, 2));
        GroupConversation conversation4 = new(group2, "convo4", messages.GetRange(4, 1));

        ConversationDocument document1 = new(conversation1.GetMessages().Select(message => message.Id).ToList(), connection1.ConnectionId, null, conversation1.ConversationId);
        ConversationDocument document2 = new(conversation2.GetMessages().Select(message => message.Id).ToList(), connection2.ConnectionId, null, conversation2.ConversationId);
        ConversationDocument document3 = new(conversation3.GetMessages().Select(message => message.Id).ToList(), null, group1.GroupId, conversation3.ConversationId);
        ConversationDocument document4 = new(conversation4.GetMessages().Select(message => message.Id).ToList(), null, group2.GroupId, conversation4.ConversationId);

        _conversationCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<ConversationDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConversationDocument>() { document1, document2, document3, document4 });

        Exception testException = new("Test Exception");

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document1, connection1, null, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation1.GetMessages()))))
            .Returns(conversation1);
        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document2, connection2, null, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation2.GetMessages()))))
            .Returns(conversation2);
        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document3, null, group1, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation3.GetMessages()))))
            .Returns(conversation3);
        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(document4, null, group2, It.Is<List<Message>>(messages => messages.SequenceEqual(conversation4.GetMessages()))))
            .Throws(testException);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationsByUserAsync(user1);

        // Then
        result.Should().BeEquivalentTo(new List<Conversation>() { conversation1, conversation2, conversation3 });
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
    public async Task GetConversationsByUser_WhenNoConversationsExist_ReturnEmptyList()
    {
        // Given
        TestUserAccount user = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
        };

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionsForUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IConnection>());
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupsByUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Group>());

        _conversationCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<ConversationDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConversationDocument>());

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationsByUserAsync(user);

        // Then
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public async Task GetConversationsByUser_WhenMongoThrowsException_ReturnEmptyListAndLogException()
    {
        // Given
        TestUserAccount user = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1",
        };

        Exception testException = new("Test Exception");

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionsForUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IConnection>());
        _groupQueryRepositoryMock
            .Setup(repo => repo.GetGroupsByUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Group>());

        _conversationCollectionMock
            .Setup(collection => collection.GetAllAsync(It.IsAny<Expression<Func<ConversationDocument, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(testException);

        // When
        var result = await _conversationQueryRepositorySUT.GetConversationsByUserAsync(user);

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
