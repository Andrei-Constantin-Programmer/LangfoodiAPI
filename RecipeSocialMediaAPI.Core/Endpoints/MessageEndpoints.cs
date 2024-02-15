using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Core.SignalR;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class MessageEndpoints
{
    public static WebApplication MapMessageEndpoints(this WebApplication app)
    {
        app.MapGroup("/message")
            .AddMessageEndpoints()
            .WithTags("Message");

        return app;
    }

    private static RouteGroupBuilder AddMessageEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get", async (
            [FromQuery] string id,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessageByIdQuery(id)));
        });

        group.MapPost("/get-detailed", async (
            [FromQuery] string id,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessageDetailedByIdQuery(id)));
        });

        group.MapPost("/get-by-conversation", async (
            [FromQuery] string conversationId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessagesByConversationQuery(conversationId)));
        });

        group.MapPost("/send", async (
            [FromBody] SendMessageContract newMessageContract,
            [FromServices] ISender sender,
            [FromServices] IHubContext<MessagingHub, IMessagingClient> context) =>
        {
            var sentMessageDto = await sender.Send(new SendMessageCommand(newMessageContract));
            await context.Clients.All.ReceiveMessage(sentMessageDto);

            return Results.Ok(sentMessageDto);
        });

        group.MapPut("/update", async (
            [FromBody] UpdateMessageContract updateMessageContract,
            [FromServices] ISender sender,
            [FromServices] IHubContext<MessagingHub, IMessagingClient> context) =>
        {
            var updatedMessage = await sender.Send(new UpdateMessageCommand(updateMessageContract));
            await context.Clients.All.ReceiveMessageUpdate(updatedMessage);

            return Results.Ok();
        });

        group.MapDelete("/delete", async (
            [FromQuery] string id,
            [FromServices] ISender sender,
            [FromServices] IHubContext<MessagingHub, IMessagingClient> context) =>
        {
            await sender.Send(new RemoveMessageCommand(id));
            await context.Clients.All.ReceiveMessageDeletion(id);

            return Results.Ok();
        });

        group.MapPut("/mark-as-read", async (
            [FromQuery] string userId,
            [FromQuery] string messageId,
            [FromServices] ISender sender,
            [FromServices] IHubContext<MessagingHub, IMessagingClient> context) =>
        {
            await sender.Send(new MarkMessageAsReadCommand(userId, messageId));
            await context.Clients.All.ReceiveMarkAsRead(userId, messageId);

            return Results.Ok();
        });

        return group;
    }


}
