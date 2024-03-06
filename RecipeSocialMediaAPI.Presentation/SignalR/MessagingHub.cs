using Microsoft.AspNetCore.SignalR;

namespace RecipeSocialMediaAPI.Presentation.SignalR;

public class MessagingHub : Hub<IMessagingClient>
{
}
