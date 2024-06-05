using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public interface IMessageNotificationService
{
    Task NotifyMessageSentAsync(MessageDto sentMessage, string conversationId, CancellationToken cancellationToken);
    Task NotifyMessageUpdatedAsync(MessageDto updatedMessage, CancellationToken cancellationToken);
    Task NotifyMessageDeletedAsync(string messageId, CancellationToken cancellationToken);
    Task NotifyMessageMarkedAsReadAsync(string userId, string messageId, CancellationToken cancellationToken);
}
