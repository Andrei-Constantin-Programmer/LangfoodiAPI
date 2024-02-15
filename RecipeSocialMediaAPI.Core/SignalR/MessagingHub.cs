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

    public async Task<MessageDTO> SendMessage(NewMessageContract contract)
    {
        var message = await _sender.Send(new SendMessageCommand(contract));
        await Clients.All.ReceiveMessage(message);

        return message;
    }

    public async Task UpdateMessage(UpdateMessageContract contract)
    {
        var updatedMessage = await _sender.Send(new UpdateMessageCommand(contract));

        await Clients.All.ReceiveMessageUpdate(updatedMessage);
    }

    public async Task DeleteMessage(string messageId)
    {
        await _sender.Send(new RemoveMessageCommand(messageId));

        await Clients.All.ReceiveMessageDeletion(messageId);
    }

    public async Task MarkMessageAsRead(string userId, string messageId)
    {
        await _sender.Send(new MarkMessageAsReadCommand(userId, messageId));

        await Clients.All.ReceiveMarkAsRead(userId, messageId);
    }
}
