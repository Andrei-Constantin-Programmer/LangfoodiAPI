using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Presentation.SignalR;

public interface IMessagingClient
{
    Task ReceiveMessage(MessageDto message, string conversationId, CancellationToken cancellationToken);
    Task ReceiveMessageUpdate(MessageDto message, CancellationToken cancellationToken);
    Task ReceiveMessageDeletion(string messageId, CancellationToken cancellationToken);
    Task ReceiveMarkAsRead(string userId, string messageId, CancellationToken cancellationToken);
}
