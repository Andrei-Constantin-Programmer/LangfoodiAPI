using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Handlers.Authentication.Querries;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;
using RecipeSocialMediaAPI.Core.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Core.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class EndpointGroups
{
    public static RouteGroupBuilder UserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/create", async (
            [FromBody] NewUserContract newUserContract,
            [FromServices] ISender sender) =>
        {
            UserDTO user = await sender.Send(new AddUserCommand(newUserContract));
            return Results.Ok(user);
        });

        group.MapPost("/update", async (
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

    public static RouteGroupBuilder RecipeEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/get", async (
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipesQuery()));
        });

        group.MapPost("/getById/{id}", async (
            [FromRoute] int id,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipeByIdQuery(id)));
        });

        group.MapPost("/create", async (
            [FromBody] RecipeDTO recipe,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new CreateRecipeCommand(recipe));
            return Results.Ok();
        });

        return group;
    }

    public static RouteGroupBuilder AuthenticationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/authenticate", async (
            [FromBody] AuthenticationAttemptContract authenticationAttempt,
            [FromServices] ISender sender) =>
        {
            var successfulLogin = await sender.Send(new AuthenticateUserQuery(authenticationAttempt.UsernameOrEmail, authenticationAttempt.Password));
            return Results.Ok(successfulLogin);
        });

        return group;
    }

    public static RouteGroupBuilder TestEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/log", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Hello World");
        });

        return group;
    }
}
