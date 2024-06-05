using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

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
            .RequireAuthorization()
            .WithDescription("Gets all conversations for a user.")
            .Produces<List<ConversationDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get-by-connection", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByConnectionQuery(userId, connectionId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets a conversation by its connection.")
            .Produces<ConversationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get-by-group", async (
            [FromQuery] string userId,
            [FromQuery] string groupId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByGroupQuery(userId, groupId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets a conversation by its group.")
            .Produces<ConversationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/create-by-connection", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateConnectionConversationCommand(userId, connectionId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Creates a conversation for a connection.")
            .Produces<ConversationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/create-by-group", async (
            [FromQuery] string userId,
            [FromQuery] string groupId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateGroupConversationCommand(userId, groupId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Creates a conversation for a group.")
            .Produces<ConversationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/mark-as-read", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new MarkConversationAsReadCommand(userId, conversationId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Marks all messages in a conversation as read for a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
