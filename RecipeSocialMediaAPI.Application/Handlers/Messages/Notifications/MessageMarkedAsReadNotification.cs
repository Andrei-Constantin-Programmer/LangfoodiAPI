using MediatR;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public record MessageMarkedAsReadNotification(string UserId, string MessageId) : INotification;

internal class MessageMarkedAsReadHandler : INotificationHandler<MessageMarkedAsReadNotification>
{
    private readonly IMessageNotificationService _messageNotificationService;

    public MessageMarkedAsReadHandler(IMessageNotificationService messageNotificationService)
    {
        _messageNotificationService = messageNotificationService;
    }

    public async Task Handle(MessageMarkedAsReadNotification notification, CancellationToken cancellationToken)
    {
        await _messageNotificationService.NotifyMessageMarkedAsRead(notification.UserId, notification.MessageId, cancellationToken);
    }
}
