using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

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
            .RequireAuthorization()
            .WithDescription("Gets a message by its id.")
            .Produces<MessageDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get-by-conversation", async (
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessagesByConversationQuery(conversationId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all messages in a conversation.")
            .Produces<List<MessageDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/send", async (
            [FromBody] SendMessageContract newMessageContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            var sentMessageDto = await sender.Send(new SendMessageCommand(newMessageContract), cancellationToken);

            return Results.Ok(sentMessageDto);
        })
            .RequireAuthorization()
            .WithDescription("Sends a message in a conversation.")
            .Produces<MessageDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/update", async (
            [FromBody] UpdateMessageContract updateMessageContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateMessageCommand(updateMessageContract), cancellationToken);

            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Updates a message.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/delete", async (
            [FromQuery] string id,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveMessageCommand(id), cancellationToken);

            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Deletes a message.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/mark-as-read", async (
            [FromQuery] string userId,
            [FromQuery] string messageId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new MarkMessageAsReadCommand(userId, messageId), cancellationToken);

            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Mark message as read for a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
