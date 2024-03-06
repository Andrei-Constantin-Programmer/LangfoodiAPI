using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

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
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConnectionQuery(userId1, userId2)));
        })
            .RequireAuthorization();

        group.MapPost("/get-by-user", async (
            [FromQuery] string userId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConnectionsByUserQuery(userId)));
        })
            .RequireAuthorization();

        group.MapPost("/create", async (
            [FromBody] NewConnectionContract newConnectionContract,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateConnectionCommand(newConnectionContract)));
        })
            .RequireAuthorization();

        group.MapPut("/update", async (
        [FromBody] UpdateConnectionContract updateConnectionContract,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateConnectionCommand(updateConnectionContract));
            return Results.Ok();
        })
            .RequireAuthorization();

        return group;
    }
}
