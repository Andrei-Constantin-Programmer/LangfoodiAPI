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
            _logger.LogWarning(ex, "SignalR notification for Message Sent has been cancelled for message with id {MessageId}", sentMessage.Id);
        }
    }

    public async Task NotifyMessageUpdated(MessageDTO updatedMessage, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.ReceiveMessageUpdate(updatedMessage, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "SignalR notification for Message Updated has been cancelled for message with id {MessageId}", updatedMessage.Id);
        }
    }
    public async Task NotifyMessageDeleted(string messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.ReceiveMessageDeletion(messageId, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "SignalR notification for Message Deleted has been cancelled for message with id {MessageId}", messageId);
        }
    }

    public async Task NotifyMessageMarkedAsRead(string userId, string messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.ReceiveMarkAsRead(userId, messageId, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "SignalR notification for Message Deleted has been cancelled for message with id {MessageId}", messageId);
        }
    }
}
