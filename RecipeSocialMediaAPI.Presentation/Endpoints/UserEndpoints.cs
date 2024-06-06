using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.DTO.Users;
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
        })
            .WithDescription("Gets all users whose handle or username contain the given string.")
            .Produces<List<UserAccountDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get-connected", async (
            [FromQuery] string userId,
            [FromQuery] string containedString,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetUsersQuery(userId, containedString, UserQueryOptions.Connected), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all connected users whose handle or username contain the given string.")
            .Produces<List<UserAccountDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get-unconnected", async (
            [FromQuery] string userId,
            [FromQuery] string containedString,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetUsersQuery(userId, containedString, UserQueryOptions.NotConnected), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all unconnected users whose handle or username contain the given string.")
            .Produces<List<UserAccountDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/create", async (
            [FromBody] NewUserContract newUserContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new AddUserCommand(newUserContract), cancellationToken));
        })
            .WithDescription("Registers a new user. Returns newly created user and a bearer token.")
            .Produces<SuccessfulAuthenticationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/update", async (
            [FromBody] UpdateUserContract updateUserContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateUserCommand(updateUserContract), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Updates a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/remove", async (
            [FromQuery] string emailOrId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveUserCommand(emailOrId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Deletes a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/username/exists", async (
            [FromQuery] string username,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CheckUsernameExistsQuery(username), cancellationToken));
        })
            .WithDescription("Checks if a user with username exists.")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/email/exists", async (
            [FromQuery] string email,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CheckEmailExistsQuery(email), cancellationToken));
        })
            .WithDescription("Checks if a user with email already exists.")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/pin", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new PinConversationCommand(userId, conversationId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Pins a conversation for a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/unpin", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UnpinConversationCommand(userId, conversationId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Unpins a conversation for a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/block", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new BlockConnectionCommand(userId, connectionId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Blocks a connection for a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/unblock", async (
            [FromQuery] string userId,
            [FromQuery] string connectionId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UnblockConnectionCommand(userId, connectionId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Unblocks a connection for a user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/pins/get", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetPinnedConversationsQuery(userId), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all pinned conversation ids for a user.")
            .Produces<List<string>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/change-role", async (
            [FromQuery] string userId,
            [FromQuery] string newRole,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new ChangeUserRoleCommand(userId, newRole), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization(IdentityData.AdminUserPolicyName)
            .WithDescription("Changes a user's role.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
