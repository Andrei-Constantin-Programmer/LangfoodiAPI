using Microsoft.AspNetCore.SignalR;

namespace RecipeSocialMediaAPI.Core.SignalR;

public class MessagingHub : Hub<IMessagingClient>
{
}
