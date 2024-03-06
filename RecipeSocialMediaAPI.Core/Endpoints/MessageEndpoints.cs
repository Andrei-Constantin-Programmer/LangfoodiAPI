using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

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
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessageByIdQuery(id), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/get-detailed", async (
            [FromQuery] string id,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessageDetailedByIdQuery(id), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/get-by-conversation", async (
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessagesByConversationQuery(conversationId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/send", async (
            [FromBody] SendMessageContract newMessageContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            var sentMessageDto = await sender.Send(new SendMessageCommand(newMessageContract), cancellationToken);

            return Results.Ok(sentMessageDto);
        })
            .RequireAuthorization();

        group.MapPut("/update", async (
            [FromBody] UpdateMessageContract updateMessageContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateMessageCommand(updateMessageContract), cancellationToken);

            return Results.Ok();
        })
            .RequireAuthorization();

        group.MapDelete("/delete", async (
            [FromQuery] string id,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveMessageCommand(id), cancellationToken);

            return Results.Ok();
        })
            .RequireAuthorization();

        group.MapPut("/mark-as-read", async (
            [FromQuery] string userId,
            [FromQuery] string messageId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new MarkMessageAsReadCommand(userId, messageId), cancellationToken);

            return Results.Ok();
        })
            .RequireAuthorization();

        return group;
    }
}
