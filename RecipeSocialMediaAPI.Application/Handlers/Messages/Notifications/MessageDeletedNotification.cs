using MediatR;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public record MessageDeletedNotification(string MessageId) : INotification;

internal class MessageDeletedHandler : INotificationHandler<MessageDeletedNotification>
{
    private readonly IMessageNotificationService _messageNotificationService;

    public MessageDeletedHandler(IMessageNotificationService messageNotificationService)
    {
        _messageNotificationService = messageNotificationService;
    }

    public async Task Handle(MessageDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _messageNotificationService.NotifyMessageDeletedAsync(notification.MessageId, cancellationToken);
    }
}
