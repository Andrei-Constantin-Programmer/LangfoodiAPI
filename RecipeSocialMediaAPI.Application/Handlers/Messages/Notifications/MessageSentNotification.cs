using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public record MessageSentNotification(MessageDto SentMessage, string ConversationId) : INotification;

internal class MessageCreatedHandler : INotificationHandler<MessageSentNotification>
{
    private readonly IMessageNotificationService _messageNotificationService;

    public MessageCreatedHandler(IMessageNotificationService messageNotificationService)
    {
        _messageNotificationService = messageNotificationService;
    }

    public async Task Handle(MessageSentNotification notification, CancellationToken cancellationToken)
    {
        await _messageNotificationService.NotifyMessageSentAsync(notification.SentMessage, notification.ConversationId, cancellationToken);
    }
}
