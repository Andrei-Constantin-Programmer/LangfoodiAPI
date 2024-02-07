using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
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

        group.MapPost("/get-by-user", async (
            [FromQuery] string userId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetGroupsByUserQuery(userId)));
        });

        group.MapPut("/update", async (
            [FromBody] UpdateGroupContract updateGroupContract,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateGroupCommand(updateGroupContract));
            return Results.Ok();
        });

        group.MapDelete("/delete", async (
            [FromQuery] string groupId,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveGroupCommand(groupId));
            return Results.Ok();
        });
        group.MapPost("/create", async (
            [FromBody] NewGroupContract newGroupContract,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateGroupCommand(newGroupContract)));
        });

        return group;
    }
}
