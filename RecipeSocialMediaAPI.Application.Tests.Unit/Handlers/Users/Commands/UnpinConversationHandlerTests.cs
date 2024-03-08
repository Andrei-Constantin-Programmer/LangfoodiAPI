using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class UnpinConversationHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IConversationQueryRepository> _conversationQueryRepository;

    private readonly UnpinConversationHandler _unpinConversationHandlerSUT;

    public UnpinConversationHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();
        _conversationQueryRepository = new Mock<IConversationQueryRepository>();

        _unpinConversationHandlerSUT = new(
            _userQueryRepositoryMock.Object,
            _userPersistenceRepositoryMock.Object,
            _conversationQueryRepository.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IUserCredentials?)null);

        UnpinConversationCommand command = new("nonExistantUserId", "convo1");

        // When
        var testAction = async () => await _unpinConversationHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConversationDoesNotExist_ThrowConversationNotFoundException()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                UserName = "User 1",
                Handler = "user_1"
            },
            Email = "user1@mail.com",
            Password = "Pass@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        UnpinConversationCommand command = new(user.Account.Id, "convo1");

        // When
        var testAction = async () => await _unpinConversationHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<ConversationNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUnpinIsSuccessful_UpdateUser()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                UserName = "User 1",
                Handler = "user_1"
            },
            Email = "user1@mail.com",
            Password = "Pass@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                UserName = "User 2",
                Handler = "user_2"
            },
            Email = "user2@mail.com",
            Password = "Pass@123"
        };
        TestUserCredentials user3 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u3",
                UserName = "User 3",
                Handler = "user_3"
            },
            Email = "user3@mail.com",
            Password = "Pass@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);

        Connection connection = new("conn1", user1.Account, user2.Account, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1");
        _conversationQueryRepository
            .Setup(repo => repo.GetConversationByIdAsync(conversation.ConversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        Connection otherConnection = new("conn2", user1.Account, user2.Account, ConnectionStatus.Connected);
        ConnectionConversation otherConversation = new(otherConnection, "convo2");

        UnpinConversationCommand command = new(user1.Account.Id, conversation.ConversationId);

        user1.Account.AddPin(conversation.ConversationId);
        user1.Account.AddPin(otherConversation.ConversationId);

        // When
        await _unpinConversationHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        user1.Account.Id.Should().Be("u1");
        user1.Account.UserName.Should().Be("User 1");
        user1.Account.Handler.Should().Be("user_1");
        user1.Email.Should().Be("user1@mail.com");
        user1.Password.Should().Be("Pass@123");
        user1.Account.PinnedConversationIds.Should().NotContain(conversation.ConversationId);
        user1.Account.PinnedConversationIds.Should().Contain(otherConversation.ConversationId);
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(user1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUnpinIsUnsuccessful_DoNotUpdateUser()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                UserName = "User 1",
                Handler = "user_1"
            },
            Email = "user1@mail.com",
            Password = "Pass@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                UserName = "User 2",
                Handler = "user_2"
            },
            Email = "user2@mail.com",
            Password = "Pass@123"
        };
        TestUserCredentials user3 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u3",
                UserName = "User 3",
                Handler = "user_3"
            },
            Email = "user3@mail.com",
            Password = "Pass@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);

        Connection connection = new("conn1", user1.Account, user2.Account, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1");
        _conversationQueryRepository
            .Setup(repo => repo.GetConversationByIdAsync(conversation.ConversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        Connection otherConnection = new("conn2", user1.Account, user2.Account, ConnectionStatus.Connected);
        ConnectionConversation otherConversation = new(otherConnection, "convo2");

        UnpinConversationCommand command = new(user1.Account.Id, conversation.ConversationId);

        user1.Account.AddPin(otherConversation.ConversationId);

        // When
        await _unpinConversationHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        user1.Account.Id.Should().Be("u1");
        user1.Account.UserName.Should().Be("User 1");
        user1.Account.Handler.Should().Be("user_1");
        user1.Email.Should().Be("user1@mail.com");
        user1.Password.Should().Be("Pass@123");
        user1.Account.PinnedConversationIds.Should().NotContain(conversation.ConversationId);
        user1.Account.PinnedConversationIds.Should().Contain(otherConversation.ConversationId);
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(user1, It.IsAny<CancellationToken>()), Times.Never);
    }
}
