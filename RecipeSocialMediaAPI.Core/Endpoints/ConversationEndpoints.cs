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
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationsByUserQuery(userId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/get-by-connection", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByConnectionQuery(userId, connectionId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/get-by-group", async (
            [FromQuery] string userId,
            [FromQuery] string groupId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByGroupQuery(userId, groupId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/create-by-connection", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateConnectionConversationCommand(userId, connectionId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/create-by-group", async (
            [FromQuery] string userId,
            [FromQuery] string groupId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateGroupConversationCommand(userId, groupId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPut("/mark-as-read", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new MarkConversationAsReadCommand(userId, conversationId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization();

        return group;
    }
}
