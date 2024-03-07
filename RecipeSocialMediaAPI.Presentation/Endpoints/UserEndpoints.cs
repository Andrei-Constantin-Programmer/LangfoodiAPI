using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;
using RecipeSocialMediaAPI.Application.Identity;
using RecipeSocialMediaAPI.Application.Utilities;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

public static class UserEndpoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapGroup("/user")
            .AddUserEndpoints()
            .WithTags("User");

        return app;
    }

    private static RouteGroupBuilder AddUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get-all", async (
            [FromQuery] string userId,
            [FromQuery] string containedString,
            [FromQuery] bool containSelf,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender
                .Send(new GetUsersQuery(userId, containedString, containSelf ? UserQueryOptions.All : UserQueryOptions.NonSelf), cancellationToken));
        });

        group.MapPost("/get-connected", async (
            [FromQuery] string userId,
            [FromQuery] string containedString,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetUsersQuery(userId, containedString, UserQueryOptions.Connected), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/get-unconnected", async (
            [FromQuery] string userId,
            [FromQuery] string containedString,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetUsersQuery(userId, containedString, UserQueryOptions.NotConnected), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/create", async (
            [FromBody] NewUserContract newUserContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new AddUserCommand(newUserContract), cancellationToken));
        });

        group.MapPut("/update", async (
            [FromBody] UpdateUserContract updateUserContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateUserCommand(updateUserContract), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization();

        group.MapDelete("/remove", async (
            [FromQuery] string emailOrId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveUserCommand(emailOrId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization();

        group.MapPost("/username/exists", async (
            [FromQuery] string username,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CheckUsernameExistsQuery(username), cancellationToken));
        });

        group.MapPost("/email/exists", async (
            [FromQuery] string email,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CheckEmailExistsQuery(email), cancellationToken));
        });

        group.MapPost("/pin", (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(sender.Send(new PinConversationCommand(userId, conversationId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/unpin", (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(sender.Send(new UnpinConversationCommand(userId, conversationId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/block", (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(sender.Send(new BlockConnectionCommand(userId, connectionId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/unblock", (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(sender.Send(new UnblockConnectionCommand(userId, connectionId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPost("/pins/get", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetPinnedConversationsQuery(userId), cancellationToken));
        })
            .RequireAuthorization();

        group.MapPut("/change-role", async(
            [FromQuery] string userId,
            [FromQuery] string newRole,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new ChangeUserRoleCommand(userId, newRole), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization(IdentityData.AdminUserPolicyName);

        return group;
    }
}
