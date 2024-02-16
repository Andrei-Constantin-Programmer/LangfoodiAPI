using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

public interface IMessageNotificationService
{
    Task NotifyMessageCreated(MessageDTO messageDTO, string conversationId, CancellationToken cancellationToken);
}
