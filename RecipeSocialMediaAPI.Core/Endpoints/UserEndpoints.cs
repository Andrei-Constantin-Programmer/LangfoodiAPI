using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

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
            [FromQuery] string containedString,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetUsersQuery(containedString)));
        })
            .RequireAuthorization();

        group.MapPost("/create", async (
            [FromBody] NewUserContract newUserContract,
            [FromServices] ISender sender) =>
        {
            UserDTO user = await sender.Send(new AddUserCommand(newUserContract));
            return Results.Ok(user);
        });

        group.MapPut("/update", async (
            [FromBody] UpdateUserContract updateUserContract,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateUserCommand(updateUserContract));
            return Results.Ok();
        })
            .RequireAuthorization();

        group.MapDelete("/remove", async (
            [FromQuery] string emailOrId,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveUserCommand(emailOrId));
            return Results.Ok();
        })
            .RequireAuthorization();

        group.MapPost("/username/exists", async (
            [FromQuery] string username,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CheckUsernameExistsQuery(username)));
        });

        group.MapPost("/email/exists", async (
            [FromQuery] string email,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CheckEmailExistsQuery(email)));
        });

        group.MapPost("/pin", (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(sender.Send(new PinConversationCommand(userId, conversationId)));
        })
            .RequireAuthorization();

        group.MapPost("/unpin", (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(sender.Send(new UnpinConversationCommand(userId, conversationId)));
        })
            .RequireAuthorization();

        group.MapPost("/pins/get", async (
            [FromQuery] string userId,
            [FromQuery] string conversationId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetPinnedConversationsQuery(userId)));
        })
            .RequireAuthorization();

        return group;
    }
}
