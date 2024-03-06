using FluentAssertions;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public partial class ConversationPersistenceRepositoryTests
{
    private readonly Mock<IConversationDocumentToModelMapper> _conversationDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<ConversationDocument>> _conversationCollectionMock;
    private readonly Mock<IMongoCollectionWrapper<ConnectionDocument>> _connectionCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;

    private readonly ConversationPersistenceRepository _conversationPersistenceRepositorySUT;

    public ConversationPersistenceRepositoryTests()
    {
        _conversationDocumentToModelMapperMock = new Mock<IConversationDocumentToModelMapper>();
        _conversationCollectionMock = new Mock<IMongoCollectionWrapper<ConversationDocument>>();
        _connectionCollectionMock = new Mock<IMongoCollectionWrapper<ConnectionDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<ConversationDocument>())
            .Returns(_conversationCollectionMock.Object);
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<ConnectionDocument>())
            .Returns(_connectionCollectionMock.Object);

        _conversationPersistenceRepositorySUT = new(_conversationDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task CreateConnectionConversation_WhenConnectionExists_CreatesConversationAndReturnsItAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Connection testConnection = new("0", user1, user2, ConnectionStatus.Pending);
        ConnectionDocument connectionDoc = new(
            Id: "connId",
            AccountId1: user1.Id,
            AccountId2: user2.Id,
            ConnectionStatus: "Pending"
        );

        Expression<Func<ConnectionDocument, bool>> expectedExpression = connDoc => 
            (connDoc.AccountId1 == user1.Id && connDoc.AccountId2 == user2.Id)
            || (connDoc.AccountId1 == user2.Id && connDoc.AccountId2 == user1.Id);

        _connectionCollectionMock
            .Setup(collection => collection.Find(
                It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectionDoc);

        ConversationDocument conversationDocument = new(
            Id: "convoId",
            ConnectionId: connectionDoc.Id,
            GroupId: null,
            Messages: new()
        );

        _conversationCollectionMock
            .Setup(collection => collection.Insert(It.Is<ConversationDocument>(doc => doc.ConnectionId == connectionDoc.Id && doc.GroupId == null)))
            .Returns(conversationDocument);

        ConnectionConversation mappedConversation = new(testConnection, conversationDocument.Id!);

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(conversationDocument, testConnection, null, It.IsAny<List<Message>>()))
            .Returns(mappedConversation);

        // When
        var result = (await _conversationPersistenceRepositorySUT.CreateConnectionConversation(testConnection)) as ConnectionConversation;

        // Then
        result.Should().NotBeNull();
        result.Should().Be(mappedConversation);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async void CreateConnectionConversation_WhenConnectionDoesNotExist_ThrowsConnectionDocumentNotFoundException()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Connection testConnection = new("0", user1, user2, ConnectionStatus.Pending);

        _connectionCollectionMock
            .Setup(collection => collection.Find(
                It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConnectionDocument?)null);

        // When
        var testAction = () => _conversationPersistenceRepositorySUT.CreateConnectionConversation(testConnection);

        // Then
        await testAction.Should().ThrowAsync<ConnectionDocumentNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateGroupConversation_CreatesConversationAndReturnsIt()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Group testGroup = new("GroupId", "Group Name", "Group Description", new[] { user1, user2 });
        
        ConversationDocument conversationDocument = new(
            Id: "convoId",
            ConnectionId: null,
            GroupId: testGroup.GroupId,
            Messages: new()
        );

        _conversationCollectionMock
            .Setup(collection => collection.Insert(It.Is<ConversationDocument>(doc => doc.GroupId == testGroup.GroupId && doc.ConnectionId == null)))
            .Returns(conversationDocument);

        GroupConversation mappedConversation = new(testGroup, conversationDocument.Id!, new List<Message>());

        _conversationDocumentToModelMapperMock
            .Setup(mapper => mapper.MapConversationFromDocument(conversationDocument, null, testGroup, It.IsAny<List<Message>>()))
            .Returns(mappedConversation);

        // When
        var result = _conversationPersistenceRepositorySUT.CreateGroupConversation(testGroup) as GroupConversation;

        // Then
        result.Should().NotBeNull();
        result.Should().Be(mappedConversation);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsConnectionConversationAndUpdateIsSuccessful_UpdatesConversationAndReturnsTrueAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Connection testConnection = new("0", user1, user2, ConnectionStatus.Pending);
        ConnectionDocument connectionDoc = new(
            Id: "ConnId",
            AccountId1: user1.Id,
            AccountId2: user2.Id,
            ConnectionStatus: "Pending"
        );
        _connectionCollectionMock
            .Setup(collection => collection.Find(
                It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectionDoc);

        ConnectionConversation testConversation = new(testConnection, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null) 
        });

        Expression<Func<ConversationDocument, bool>> expectedExpression = convoDoc => convoDoc.Id == testConversation.ConversationId;

        _conversationCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.Is<ConversationDocument>(convoDoc => 
                    convoDoc.Id == testConversation.ConversationId
                    && convoDoc.ConnectionId == connectionDoc.Id
                    && convoDoc.GroupId == null
                    && convoDoc.Messages.SequenceEqual(testConversation.Messages.Select(m => m.Id))), 
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(true);

        // When
        var result = await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, testConnection);

        // Then
        result.Should().BeTrue();
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsConnectionConversationButUpdateIsUnsuccessful_ReturnsFalseAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Connection testConnection = new("0", user1, user2, ConnectionStatus.Pending);
        ConnectionDocument connectionDoc = new(
            Id: "ConnId",
            AccountId1: user1.Id,
            AccountId2: user2.Id,
            ConnectionStatus: "Pending"
        );

        _connectionCollectionMock
            .Setup(collection => collection.Find(
                It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectionDoc);

        ConnectionConversation testConversation = new(testConnection, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        _conversationCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<ConversationDocument>(),
                It.IsAny<Expression<Func<ConversationDocument, bool>>>()))
            .Returns(false);

        // When
        var result = await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, testConnection);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsConnectionConversationButConnectionIsNotFoundInTheDatabase_DoesNotUpdateAndThrowsInvalidConversationExceptionAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Connection testConnection = new("0", user1, user2, ConnectionStatus.Pending);
        _connectionCollectionMock
            .Setup(collection => collection.Find(
                It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConnectionDocument?)null);

        ConnectionConversation testConversation = new(testConnection, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        // When
        var testAction = async () => await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, testConnection);

        // Then
        await testAction.Should()
            .ThrowAsync<InvalidConversationException>()
            .WithMessage("No connection found*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsConnectionConversationButNoConnectionSupplied_DoesNotUpdateConversationAndThrowsArgumentExceptionAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Connection testConnection = new("0", user1, user2, ConnectionStatus.Pending);
        ConnectionDocument connectionDoc = new(
            Id: "ConnId",
            AccountId1: user1.Id,
            AccountId2: user2.Id,
            ConnectionStatus: "Pending"
        );
        _connectionCollectionMock
            .Setup(collection => collection.Find(
                It.IsAny<Expression<Func<ConnectionDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectionDoc);

        ConnectionConversation testConversation = new(testConnection, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        // When
        var testAction = async () => await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation);

        // Then
        await testAction.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("No connection provided*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsGroupConversationAndUpdateIsSuccessful_UpdatesConversationAndReturnsTrueAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Group testGroup = new("GroupId", "Group Name", "Group Description", new[] { user1, user2 });
        GroupConversation testConversation = new(testGroup, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        Expression<Func<ConversationDocument, bool>> expectedExpression = convoDoc => convoDoc.Id == testConversation.ConversationId;

        _conversationCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.Is<ConversationDocument>(convoDoc =>
                    convoDoc.Id == testConversation.ConversationId
                    && convoDoc.ConnectionId == null
                    && convoDoc.GroupId == testGroup.GroupId
                    && convoDoc.Messages.SequenceEqual(testConversation.Messages.Select(m => m.Id))),
                It.Is<Expression<Func<ConversationDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(true);

        // When
        var result = await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, null, testGroup);

        // Then
        result.Should().BeTrue();
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsGroupConversationButUpdateIsUnsuccessful_ReturnsFalseAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Group testGroup = new("GroupId", "Group Name", "Group Description", new[] { user1, user2 });
        GroupConversation testConversation = new(testGroup, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        _conversationCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<ConversationDocument>(),
                It.IsAny<Expression<Func<ConversationDocument, bool>>>()))
            .Returns(false);

        // When
        var result = await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, null, testGroup);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsGroupConversationButNoGroupIsProvided_DoesNotUpdateConversationAndThrowsArgumentExceptionAsync()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "UserId1",
            Handler = "user1",
            UserName = "UserName1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount user2 = new()
        {
            Id = "UserId2",
            Handler = "user2",
            UserName = "UserName2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

        Group testGroup = new("GroupId", "Group Name", "Group Description", new[] { user1, user2 });
        GroupConversation testConversation = new(testGroup, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        // When
        var testAction = async () => await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, null);

        // Then
        await testAction.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("No group provided*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateConversation_WhenConversationIsOfUnknownType_DoesNotUpdateConversationAndThrowsInvalidConversationExceptionAsync()
    {
        // Given
        TestConversation testConversation = new("ConvoId", new List<Message>());
        
        // When
        var testAction = async () => await _conversationPersistenceRepositorySUT.UpdateConversation(testConversation);

        // Then
        await testAction.Should()
            .ThrowAsync<InvalidConversationException>()
            .WithMessage("*TestConversation*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }
}
