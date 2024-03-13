using FluentAssertions;
using MediatR;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class MarkMessageAsReadHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;
    private readonly Mock<IPublisher> _publisherMock;

    private readonly MarkMessageAsReadHandler _markMessageAsReadHandlerSUT;

    public MarkMessageAsReadHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();
        _publisherMock = new Mock<IPublisher>();

        _markMessageAsReadHandlerSUT = new(
            _userQueryRepositoryMock.Object,
            _messageQueryRepositoryMock.Object,
            _messagePersistenceRepositoryMock.Object,
            _publisherMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessageIsFound_MarkMessageAsRead()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "user1@mail.com",
            Password = "Test@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                Handler = "user_2",
                UserName = "User 2"
            },
            Email = "user2@mail.com",
            Password = "Test@123"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        TestMessage message = new("m1", user1.Account, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, seenBy: new List<IUserAccount> { user1.Account });
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(message.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        
        MarkMessageAsReadCommand command = new(user2.Account.Id, message.Id);

        // When
        await _markMessageAsReadHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(
                    It.Is<Message>(m
                        => m.Id == message.Id
                        && m.Sender == message.Sender
                        && m.SeenBy.SequenceEqual(new List<IUserAccount> { user1.Account, user2.Account })
                        && m.SentDate == message.SentDate
                        && m.UpdatedDate == message.UpdatedDate), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessageIsNotFound_ThrowMessageNotFoundException()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "user1@mail.com",
            Password = "Test@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                Handler = "user_2",
                UserName = "User 2"
            },
            Email = "user2@mail.com",
            Password = "Test@123"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message?)null);
        
        MarkMessageAsReadCommand command = new(user2.Account.Id, "m1");

        // When
        var testAction = async () => await _markMessageAsReadHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<MessageNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserIsNotFound_ThrowUserNotFoundException()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "user1@mail.com",
            Password = "Test@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IUserCredentials?)null);

        MarkMessageAsReadCommand command = new("u1", "m1");

        // When
        var testAction = async () => await _markMessageAsReadHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessageIsMarkedSuccessfully_NotifyMessageMarkedAsRead()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "user1@mail.com",
            Password = "Test@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                Handler = "user_2",
                UserName = "User 2"
            },
            Email = "user2@mail.com",
            Password = "Test@123"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        TestMessage message = new("m1", user1.Account, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, seenBy: new List<IUserAccount> { user1.Account });
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(message.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);

        MarkMessageAsReadCommand command = new(user2.Account.Id, message.Id);

        // When
        await _markMessageAsReadHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        _publisherMock
            .Verify(publisher => publisher.Publish(
                    It.Is<MessageMarkedAsReadNotification>(notification
                        => notification.UserId == user2.Account.Id
                        && notification.MessageId == message.Id), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }
}
