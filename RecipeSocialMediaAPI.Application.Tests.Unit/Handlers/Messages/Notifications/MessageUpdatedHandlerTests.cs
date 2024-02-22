using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Notifications;

public class MessageUpdatedHandlerTests
{
    private readonly Mock<IMessageNotificationService> _messageNotificationServiceMock;

    private readonly MessageUpdatedHandler _messageUpdatedHandlerSUT;

    public MessageUpdatedHandlerTests()
    {
        _messageNotificationServiceMock = new Mock<IMessageNotificationService>();

        _messageUpdatedHandlerSUT = new(_messageNotificationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationNotTriggered_NotifyMessageUpdated()
    {
        // Given
        MessageDTO message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");
        MessageUpdatedNotification notification = new(message);

        // When
        await _messageUpdatedHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        _messageNotificationServiceMock
            .Verify(service => service.NotifyMessageUpdated(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationIsTriggered_ThrowOperationCanceledException()
    {
        // Given
        MessageDTO message = new("m1", new("u1", "User 1"), new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");
        MessageUpdatedNotification notification = new(message);

        _messageNotificationServiceMock
            .Setup(service => service.NotifyMessageUpdated(message, It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        var testAction = async () => await _messageUpdatedHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<OperationCanceledException>();
    }
}
