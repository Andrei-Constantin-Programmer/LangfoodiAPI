using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public interface IMessageNotificationService
{
    Task NotifyMessageSent(MessageDTO sentMessage, string conversationId, CancellationToken cancellationToken);
}
