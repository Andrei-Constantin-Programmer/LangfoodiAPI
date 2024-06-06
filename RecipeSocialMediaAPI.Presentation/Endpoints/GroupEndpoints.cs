using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Identity;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

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
            .RequireAuthorization()
            .WithDescription("Gets a group by its id.")
            .Produces<GroupDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get-by-user", async (
            [FromQuery] string userId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetGroupsByUserQuery(userId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all groups for a user.")
            .Produces<List<GroupDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/create", async (
            [FromBody] NewGroupContract newGroupContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateGroupCommand(newGroupContract), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Creates a group.")
            .Produces<GroupDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/update", async (
            [FromBody] UpdateGroupContract updateGroupContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateGroupCommand(updateGroupContract), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Updates a group.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/delete", async (
            [FromQuery] string groupId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveGroupCommand(groupId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization(IdentityData.DeveloperUserPolicyName)
            .WithDescription("Deletes a group.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
