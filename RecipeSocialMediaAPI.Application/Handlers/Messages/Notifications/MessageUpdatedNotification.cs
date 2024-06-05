using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public record MessageUpdatedNotification(MessageDto UpdatedMessage) : INotification;

internal class MessageUpdatedHandler : INotificationHandler<MessageUpdatedNotification>
{
    private readonly IMessageNotificationService _messageNotificationService;

    public MessageUpdatedHandler(IMessageNotificationService messageNotificationService)
    {
        _messageNotificationService = messageNotificationService;
    }

    public async Task Handle(MessageUpdatedNotification notification, CancellationToken cancellationToken)
    {
        await _messageNotificationService.NotifyMessageUpdatedAsync(notification.UpdatedMessage, cancellationToken);
    }
}
