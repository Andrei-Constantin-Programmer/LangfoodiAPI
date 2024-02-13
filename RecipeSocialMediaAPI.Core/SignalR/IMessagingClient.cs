using RecipeSocialMediaAPI.Application.DTO.Message;

namespace RecipeSocialMediaAPI.Core.SignalR;

public interface IMessagingClient
{
    Task ReceiveMessage(MessageDTO message);
    Task ReceiveMessageUpdate(MessageDTO message);
    Task ReceiveMessageDeletion(string messageId);
    Task ReceiveMarkAsRead(string userId, string messageId);
}
