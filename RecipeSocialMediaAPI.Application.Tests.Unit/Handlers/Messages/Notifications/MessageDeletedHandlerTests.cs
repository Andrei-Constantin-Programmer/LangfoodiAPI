using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Notifications;

public class MessageDeletedHandlerTests
{
    private readonly Mock<IMessageNotificationService> _messageNotificationServiceMock;

    private readonly MessageDeletedHandler _messageDeletedHandlerSUT;

    public MessageDeletedHandlerTests()
    {
        _messageNotificationServiceMock = new Mock<IMessageNotificationService>();

        _messageDeletedHandlerSUT = new(_messageNotificationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationNotTriggered_NotifyMessageDeleted()
    {
        // Given
        string messageId = "m1";
        MessageDeletedNotification notification = new(messageId);

        // When
        await _messageDeletedHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        _messageNotificationServiceMock
            .Verify(service => service.NotifyMessageDeleted(messageId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationIsTriggered_ThrowOperationCanceledException()
    {
        // Given
        string messageId = "m1";
        MessageDeletedNotification notification = new(messageId);

        _messageNotificationServiceMock
            .Setup(service => service.NotifyMessageDeleted(messageId, It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        var testAction = async () => await _messageDeletedHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<OperationCanceledException>();
    }
}
