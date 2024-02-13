using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class ConversationEndpoints
{
    public static WebApplication MapConversationEndpoints(this WebApplication app)
    {
        app.MapGroup("/conversation")
            .AddConversationEndpoints()
            .WithTags("Conversation");

        return app;
    }

    private static RouteGroupBuilder AddConversationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get-by-user", async (
            [FromQuery] string userId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationsByUserQuery(userId)));
        });

        group.MapPost("/get-by-connection", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByConnectionQuery(userId, connectionId)));
        });

        group.MapPost("/get-by-group", async (
            [FromQuery] string userId,
            [FromQuery] string groupId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByGroupQuery(userId, groupId)));
        });

        group.MapPost("/create-by-connection", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateConnectionConversationCommand(userId, connectionId)));
        });

        group.MapPost("/create-by-group", async (
            [FromQuery] string userId,
            [FromQuery] string groupId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateGroupConversationCommand(userId, groupId)));
        });

        group.MapPut("/mark-as-read", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new MarkConversationAsReadCommand(userId, conversationId));
            return Results.Ok();
        });

        return group;
    }
}
