using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.SignalR;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Core.Tests.Unit.SignalR;

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
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task NotifyMessageCreated_WhenCancellationIsNotTriggered_NotifyAllClientsOfNewMessage()
    {
        // Given
        string conversationId = "convo1";
        MessageDTO message = new("m1", "u1", "User 1", new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");

        // When
        await _messageNotificationServiceSUT.NotifyMessageCreated(message, conversationId, CancellationToken.None);

        // Then
        _hubContextMock
            .Verify(context => context.Clients.All.ReceiveMessage(message, conversationId, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task NotifyMessageCreated_WhenCancellationIsTriggered_LogsWarning()
    {
        // Given
        string conversationId = "convo1";
        MessageDTO message = new("m1", "u1", "User 1", new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");

        _hubContextMock
            .Setup(context => context.Clients.All.ReceiveMessage(It.IsAny<MessageDTO>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        await _messageNotificationServiceSUT.NotifyMessageCreated(message, conversationId, new CancellationToken(true));

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
