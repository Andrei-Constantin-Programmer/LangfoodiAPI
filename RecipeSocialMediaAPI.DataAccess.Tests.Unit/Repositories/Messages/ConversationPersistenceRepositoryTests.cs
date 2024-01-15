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
    public void CreateConnectionConversation_WhenConnectionExists_CreatesConversationAndReturnsIt()
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

        Connection testConnection = new(user1, user2, ConnectionStatus.Pending);
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
            .Setup(collection => collection.Find(It.Is<Expression<Func<ConnectionDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression))))
            .Returns(connectionDoc);

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
        var result = _conversationPersistenceRepositorySUT.CreateConnectionConversation(testConnection) as ConnectionConversation;

        // Then
        result.Should().NotBeNull();
        result.Should().Be(mappedConversation);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void CreateConnectionConversation_WhenConnectionDoesNotExist_ThrowsConnectionDocumentNotFoundException()
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

        Connection testConnection = new(user1, user2, ConnectionStatus.Pending);

        _connectionCollectionMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<ConnectionDocument, bool>>>()))
            .Returns((ConnectionDocument?)null);

        // When
        var testAction = () => _conversationPersistenceRepositorySUT.CreateConnectionConversation(testConnection);

        // Then
        testAction.Should().Throw<ConnectionDocumentNotFoundException>();
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
    public void UpdateConversation_WhenConversationIsConnectionConversationAndUpdateIsSuccessful_UpdatesConversationAndReturnsTrue()
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

        Connection testConnection = new(user1, user2, ConnectionStatus.Pending);
        ConnectionDocument connectionDoc = new(
            Id: "ConnId",
            AccountId1: user1.Id,
            AccountId2: user2.Id,
            ConnectionStatus: "Pending"
        );
        _connectionCollectionMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<ConnectionDocument, bool>>>()))
            .Returns(connectionDoc);

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
        var result = _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, testConnection);

        // Then
        result.Should().BeTrue();
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConversation_WhenConversationIsConnectionConversationButUpdateIsUnsuccessful_ReturnsFalse()
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

        Connection testConnection = new(user1, user2, ConnectionStatus.Pending);
        ConnectionDocument connectionDoc = new(
            Id: "ConnId",
            AccountId1: user1.Id,
            AccountId2: user2.Id,
            ConnectionStatus: "Pending"
        );

        _connectionCollectionMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<ConnectionDocument, bool>>>()))
            .Returns(connectionDoc);

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
        var result = _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, testConnection);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConversation_WhenConversationIsConnectionConversationButConnectionIsNotFoundInTheDatabase_DoesNotUpdateAndThrowsInvalidConversationException()
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

        Connection testConnection = new(user1, user2, ConnectionStatus.Pending);
        _connectionCollectionMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<ConnectionDocument, bool>>>()))
            .Returns((ConnectionDocument?)null);

        ConnectionConversation testConversation = new(testConnection, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        // When
        var testAction = () => _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, testConnection);

        // Then
        testAction.Should()
            .Throw<InvalidConversationException>()
            .WithMessage("No connection found*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConversation_WhenConversationIsConnectionConversationButNoConnectionSupplied_DoesNotUpdateConversationAndThrowsArgumentException()
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

        Connection testConnection = new(user1, user2, ConnectionStatus.Pending);
        ConnectionDocument connectionDoc = new(
            Id: "ConnId",
            AccountId1: user1.Id,
            AccountId2: user2.Id,
            ConnectionStatus: "Pending"
        );
        _connectionCollectionMock
            .Setup(collection => collection.Find(It.IsAny<Expression<Func<ConnectionDocument, bool>>>()))
            .Returns(connectionDoc);

        ConnectionConversation testConversation = new(testConnection, "ConvoId", new List<Message>()
        {
            new TestMessage("MsgId", user1, new(2023, 11, 15, 0, 0, 0, TimeSpan.Zero), null)
        });

        // When
        var testAction = () => _conversationPersistenceRepositorySUT.UpdateConversation(testConversation);

        // Then
        testAction.Should()
            .Throw<ArgumentException>()
            .WithMessage("No connection provided*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConversation_WhenConversationIsGroupConversationAndUpdateIsSuccessful_UpdatesConversationAndReturnsTrue()
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
        var result = _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, null, testGroup);

        // Then
        result.Should().BeTrue();
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConversation_WhenConversationIsGroupConversationButUpdateIsUnsuccessful_ReturnsFalse()
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
        var result = _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, null, testGroup);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConversation_WhenConversationIsGroupConversationButNoGroupIsProvided_DoesNotUpdateConversationAndThrowsArgumentException()
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
        var testAction = () => _conversationPersistenceRepositorySUT.UpdateConversation(testConversation, null);

        // Then
        testAction.Should()
            .Throw<ArgumentException>()
            .WithMessage("No group provided*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void UpdateConversation_WhenConversationIsOfUnknownType_DoesNotUpdateConversationAndThrowsInvalidConversationException()
    {
        // Given
        TestConversation testConversation = new("ConvoId", new List<Message>());
        
        // When
        var testAction = () => _conversationPersistenceRepositorySUT.UpdateConversation(testConversation);

        // Then
        testAction.Should()
            .Throw<InvalidConversationException>()
            .WithMessage("*TestConversation*");
        _conversationCollectionMock
            .Verify(collection => collection.UpdateRecord(It.IsAny<ConversationDocument>(), It.IsAny<Expression<Func<ConversationDocument, bool>>>()), Times.Never);
    }
}
