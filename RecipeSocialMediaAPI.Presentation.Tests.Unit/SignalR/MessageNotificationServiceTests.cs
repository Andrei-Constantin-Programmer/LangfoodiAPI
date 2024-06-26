﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Presentation.SignalR;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Presentation.Tests.Unit.SignalR;

public class MessageNotificationServiceTests
{
    private readonly Mock<IHubContext<MessagingHub, IMessagingClient>> _hubContextMock;
    private readonly Mock<ILogger<MessageNotificationService>> _loggerMock;

    private readonly MessageNotificationService _messageNotificationServiceSUT;

    public MessageNotificationServiceTests()
    {
        _hubContextMock = new Mock<IHubContext<MessagingHub, IMessagingClient>>();
        _loggerMock = new Mock<ILogger<MessageNotificationService>>();

        Mock<IMessagingClient> messagingClientMock = new();
        Mock<IHubClients<IMessagingClient>> hubClientsMock = new();
        hubClientsMock
            .Setup(clients => clients.All)
            .Returns(messagingClientMock.Object);
        _hubContextMock
            .Setup(context => context.Clients)
            .Returns(hubClientsMock.Object);

        _messageNotificationServiceSUT = new(_hubContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageSent_WhenCancellationIsNotTriggered_NotifyAllClientsOfNewMessage()
    {
        // Given
        string conversationId = "convo1";
        MessageDto message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");

        // When
        await _messageNotificationServiceSUT.NotifyMessageSentAsync(message, conversationId, CancellationToken.None);

        // Then
        _hubContextMock
            .Verify(context => context.Clients.All.ReceiveMessage(message, conversationId, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageSent_WhenCancellationIsTriggered_LogsWarning()
    {
        // Given
        string conversationId = "convo1";
        MessageDto message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");

        _hubContextMock
            .Setup(context => context.Clients.All.ReceiveMessage(It.IsAny<MessageDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        await _messageNotificationServiceSUT.NotifyMessageSentAsync(message, conversationId, new CancellationToken(true));

        // Then
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<OperationCanceledException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageUpdated_WhenCancellationIsNotTriggered_NotifyAllClientsOfMessageUpdate()
    {
        // Given
        MessageDto message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");

        // When
        await _messageNotificationServiceSUT.NotifyMessageUpdatedAsync(message, CancellationToken.None);

        // Then
        _hubContextMock
            .Verify(context => context.Clients.All.ReceiveMessageUpdate(message, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageUpdated_WhenCancellationIsTriggered_LogsWarning()
    {
        // Given
        MessageDto message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");

        _hubContextMock
            .Setup(context => context.Clients.All.ReceiveMessageUpdate(It.IsAny<MessageDto>(), It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        await _messageNotificationServiceSUT.NotifyMessageUpdatedAsync(message, new CancellationToken(true));

        // Then
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<OperationCanceledException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageDeleted_WhenCancellationIsNotTriggered_NotifyAllClientsOfMessageDeletion()
    {
        // Given
        string messageId = "m1";

        // When
        await _messageNotificationServiceSUT.NotifyMessageDeletedAsync(messageId, CancellationToken.None);

        // Then
        _hubContextMock
            .Verify(context => context.Clients.All.ReceiveMessageDeletion(messageId, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageDeleted_WhenCancellationIsTriggered_LogsWarning()
    {
        // Given
        string messageId = "m1";

        _hubContextMock
            .Setup(context => context.Clients.All.ReceiveMessageDeletion(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        await _messageNotificationServiceSUT.NotifyMessageDeletedAsync(messageId, new CancellationToken(true));

        // Then
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<OperationCanceledException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageMarkedAsRead_WhenCancellationIsNotTriggered_NotifyAllClientsOfMessageMarkedAsRead()
    {
        // Given
        string userId = "u1";
        string messageId = "m1";

        // When
        await _messageNotificationServiceSUT.NotifyMessageMarkedAsReadAsync(userId, messageId, CancellationToken.None);

        // Then
        _hubContextMock
            .Verify(context => context.Clients.All.ReceiveMarkAsRead(userId, messageId, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task NotifyMessageMarkedAsRead_WhenCancellationIsTriggered_LogsWarning()
    {
        // Given
        string userId = "u1";
        string messageId = "m1";

        _hubContextMock
            .Setup(context => context.Clients.All.ReceiveMarkAsRead(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        await _messageNotificationServiceSUT.NotifyMessageMarkedAsReadAsync(userId, messageId, new CancellationToken(true));

        // Then
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<OperationCanceledException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }
}
