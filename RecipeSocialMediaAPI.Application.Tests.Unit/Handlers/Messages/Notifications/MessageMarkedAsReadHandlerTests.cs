using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Notifications;

public class MessageMarkedAsReadHandlerTests
{
    private readonly Mock<IMessageNotificationService> _messageNotificationServiceMock;

    private readonly MessageMarkedAsReadHandler _messageMarkedAsReadHandlerSUT;

    public MessageMarkedAsReadHandlerTests()
    {
        _messageNotificationServiceMock = new Mock<IMessageNotificationService>();

        _messageMarkedAsReadHandlerSUT = new(_messageNotificationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationNotTriggered_NotifyMessageMarkedAsRead()
    {
        // Given
        string userId = "u1";
        string messageId = "m1";
        MessageMarkedAsReadNotification notification = new(userId, messageId);

        // When
        await _messageMarkedAsReadHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        _messageNotificationServiceMock
            .Verify(service => service.NotifyMessageMarkedAsReadAsync(userId, messageId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationIsTriggered_ThrowOperationCanceledException()
    {
        // Given
        string userId = "u1";
        string messageId = "m1";
        MessageMarkedAsReadNotification notification = new(userId, messageId);

        _messageNotificationServiceMock
            .Setup(service => service.NotifyMessageMarkedAsReadAsync(userId, messageId, It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        var testAction = async () => await _messageMarkedAsReadHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<OperationCanceledException>();
    }
}
