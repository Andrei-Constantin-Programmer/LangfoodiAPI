using Microsoft.AspNetCore.SignalR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;

namespace RecipeSocialMediaAPI.Core.SignalR;

public class MessageNotificationService : IMessageNotificationService
{
    private readonly IHubContext<MessagingHub, IMessagingClient> _hubContext;
    private readonly ILogger<MessageNotificationService> _logger;

    public MessageNotificationService(IHubContext<MessagingHub, IMessagingClient> hubContext, ILogger<MessageNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyMessageSent(MessageDTO sentMessage, string conversationId, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.ReceiveMessage(sentMessage, conversationId, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "SignalR notification for Message Created has been cancelled for message with id {MessageId}", sentMessage.Id);
        }
    }
}
