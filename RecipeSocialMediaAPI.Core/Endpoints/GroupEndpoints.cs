using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class GroupEndpoints
{
    public static WebApplication MapGroupEndpoints(this WebApplication app)
    {
        app.MapGroup("/group")
            .AddGroupEndpoints()
            .WithTags("Group");

        return app;
    }

    private static RouteGroupBuilder AddGroupEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get", async (
            [FromQuery] string groupId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetGroupQuery(groupId)));
        });

        return group;
    }
}
