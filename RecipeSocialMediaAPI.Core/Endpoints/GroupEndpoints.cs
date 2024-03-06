using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Identity;

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
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetGroupQuery(groupId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/get-by-user", async (
            [FromQuery] string userId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetGroupsByUserQuery(userId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPut("/update", async (
            [FromBody] UpdateGroupContract updateGroupContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateGroupCommand(updateGroupContract), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization();

        group.MapDelete("/delete", async (
            [FromQuery] string groupId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveGroupCommand(groupId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization(IdentityData.DeveloperUserPolicyName);

        group.MapPost("/create", async (
            [FromBody] NewGroupContract newGroupContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateGroupCommand(newGroupContract), cancellationToken));
        })
            .RequireAuthorization();

        return group;
    }
}
