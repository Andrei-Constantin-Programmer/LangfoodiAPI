using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Notifications;

public class MessageCreatedHandlerTests
{
    private readonly Mock<IMessageNotificationService> _messageNotificationServiceMock;

    private readonly MessageCreatedHandler _messageCreatedHandlerSUT;

    public MessageCreatedHandlerTests()
    {
        _messageNotificationServiceMock = new Mock<IMessageNotificationService>();

        _messageCreatedHandlerSUT = new(_messageNotificationServiceMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationNotTriggered_NotifyMessageCreated()
    {
        // Given
        string conversationId = "convo1";
        MessageDTO message = new("m1", "u1", "User 1", new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");
        MessageCreatedNotification notification = new(message, conversationId);

        // When
        await _messageCreatedHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        _messageNotificationServiceMock
            .Verify(service => service.NotifyMessageCreated(message, conversationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenCancellationIsTriggered_ThrowOperationCanceledException()
    {
        // Given
        string conversationId = "convo1";
        MessageDTO message = new("m1", "u1", "User 1", new() { "u1" }, new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TextContent: "text");
        MessageCreatedNotification notification = new(message, conversationId);

        _messageNotificationServiceMock
            .Setup(service => service.NotifyMessageCreated(message, conversationId, It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // When
        var testAction = async () => await _messageCreatedHandlerSUT.Handle(notification, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<OperationCanceledException>();
    }
}
