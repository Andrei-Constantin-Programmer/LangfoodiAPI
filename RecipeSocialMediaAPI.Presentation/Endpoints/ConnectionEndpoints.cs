using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Identity;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

public static class ConnectionEndpoints
{
    public static WebApplication MapConnectionEndpoints(this WebApplication app)
    {
        app.MapGroup("/connection")
            .AddConnectionEndpoints()
            .WithTags("Connection");

        return app;
    }

    private static RouteGroupBuilder AddConnectionEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get", async (
            [FromQuery] string userId1,
            [FromQuery] string userId2,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConnectionQuery(userId1, userId2), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets connection between two users.")
            .Produces<ConnectionDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get-by-user", async (
            [FromQuery] string userId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConnectionsByUserQuery(userId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all connections for user.")
            .Produces<List<ConnectionDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/create", async (
            [FromBody] NewConnectionContract newConnectionContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateConnectionCommand(newConnectionContract), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Creates new connection between two users.")
            .Produces<ConnectionDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/update", async (
            [FromBody] UpdateConnectionContract updateConnectionContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateConnectionCommand(updateConnectionContract), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Updates a connection.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/delete", async (
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveConnectionCommand(connectionId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization(IdentityData.DeveloperUserPolicyName)
            .WithDescription("Deletes a connection.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
