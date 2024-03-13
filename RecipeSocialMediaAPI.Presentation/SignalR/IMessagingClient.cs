using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Presentation.SignalR;

public interface IMessagingClient
{
    Task ReceiveMessage(MessageDTO message, string conversationId, CancellationToken cancellationToken);
    Task ReceiveMessageUpdate(MessageDTO message, CancellationToken cancellationToken);
    Task ReceiveMessageDeletion(string messageId, CancellationToken cancellationToken);
    Task ReceiveMarkAsRead(string userId, string messageId, CancellationToken cancellationToken);
}
