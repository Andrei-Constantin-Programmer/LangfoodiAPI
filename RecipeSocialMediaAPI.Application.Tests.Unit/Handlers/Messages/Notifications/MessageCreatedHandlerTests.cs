using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Notifications;

public class MessageCreatedHandlerTests
{
    private readonly Mock<IMessageNotificationService> _messageNotificationServiceMock;

    private readonly MessageCreatedHandler _messageSentHandlerSUT;

    public MessageCreatedHandlerTests()
    {
        _messageNotificationServiceMock = new Mock<IMessageNotificationService>();

        _messageSentHandlerSUT = new(_messageNotificationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationNotTriggered_NotifyMessageSent()
    {
        // Given
        string conversationId = "convo1";
        MessageDto message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");
        MessageSentNotification notification = new(message, conversationId);

        // When
        await _messageSentHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        _messageNotificationServiceMock
            .Verify(service => service.NotifyMessageSentAsync(message, conversationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationIsTriggered_ThrowOperationCanceledException()
    {
        // Given
        string conversationId = "convo1";
        MessageDto message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");
        MessageSentNotification notification = new(message, conversationId);

        _messageNotificationServiceMock
            .Setup(service => service.NotifyMessageSentAsync(message, conversationId, It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        var testAction = async () => await _messageSentHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<OperationCanceledException>();
    }
}
