using MediatR;
using Microsoft.AspNetCore.SignalR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

namespace RecipeSocialMediaAPI.Core.SignalR;

public class MessagingHub : Hub<IMessagingClient>
{
    private readonly ISender _sender;

    public MessagingHub(ISender sender)
    {
        _sender = sender;
    }

    public async Task<MessageDTO> SendMessage(SendMessageContract SendMessageContract)
    {
        var message = await _sender.Send(new SendMessageCommand(SendMessageContract));
        await Clients.All.ReceiveMessage(message);

        return message;
    }

    public async Task UpdateMessage(UpdateMessageContract UpdateMessageContract)
    {
        var updatedMessage = await _sender.Send(new UpdateMessageCommand(UpdateMessageContract));

        await Clients.All.ReceiveMessageUpdate(updatedMessage);
    }

    public async Task DeleteMessage(string MessageId)
    {
        await _sender.Send(new RemoveMessageCommand(MessageId));

        await Clients.All.ReceiveMessageDeletion(MessageId);
    }

    public async Task MarkMessageAsRead(string UserId, string MessageId)
    {
        await _sender.Send(new MarkMessageAsReadCommand(UserId, MessageId));

        await Clients.All.ReceiveMarkAsRead(UserId, MessageId);
    }
}
