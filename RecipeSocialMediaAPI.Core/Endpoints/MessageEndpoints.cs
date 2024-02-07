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

        group.MapPost("/send", async (
            [FromBody] NewMessageContract newMessageContract,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new SendMessageCommand(newMessageContract)));
        });

        group.MapPut("/update", async (
            [FromBody] UpdateMessageContract updateMessageContract,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateMessageCommand(updateMessageContract));
            return Results.Ok();
        });

        group.MapDelete("/delete", async (
            [FromQuery] string id,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveMessageCommand(id));
            return Results.Ok();
        });

        group.MapPut("/mark-as-read", async (
            [FromQuery] string userId,
            [FromQuery] string messageId,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new MarkMessageAsReadCommand(userId, messageId));
            return Results.Ok();
        });

        return group;
    }


}
