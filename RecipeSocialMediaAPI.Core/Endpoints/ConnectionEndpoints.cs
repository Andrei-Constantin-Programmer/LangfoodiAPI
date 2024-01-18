using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class ConnectionEndpoints
{
    public static void MapConnectionEndpoints(this WebApplication app)
    {
        app.MapGroup("/connection")
            .AddConnectionEndpoints()
            .WithTags("Connection");
    }

    private static RouteGroupBuilder AddConnectionEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get/userIds", async (
            [FromQuery] string userId1,
            [FromQuery] string userId2,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConnectionQuery(userId1, userId2)));
        });

        return group;
    }
}
