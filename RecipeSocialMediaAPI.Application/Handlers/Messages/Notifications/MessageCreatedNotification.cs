using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public record MessageCreatedNotification(MessageDTO Message, string ConversationId) : INotification;

internal class MessageCreatedHandler : INotificationHandler<MessageCreatedNotification>
{
    private readonly IMessageNotificationService _messageNotificationService;

    public MessageCreatedHandler(IMessageNotificationService messageNotificationService)
    {
        _messageNotificationService = messageNotificationService;
    }

    public async Task Handle(MessageCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _messageNotificationService.NotifyMessageCreated(notification.Message, notification.ConversationId, cancellationToken);
    }
}
