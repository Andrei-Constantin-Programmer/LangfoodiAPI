using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGroup("/user")
            .AddUserEndpoints()
            .WithTags("User");
    }

    private static RouteGroupBuilder AddUserEndpoints(this RouteGroupBuilder group)
    {
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
        });

        group.MapDelete("/remove", async (
            [FromQuery] string emailOrId,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveUserCommand(emailOrId));
            return Results.Ok();
        });

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

        return group;
    }
}
