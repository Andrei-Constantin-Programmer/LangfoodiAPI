using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/users/createuser", async (
            [FromBody] UserDto newUser, 
            [FromServices] ISender sender) =>
        {
            try
            {
                return Results.Ok(await sender.Send(new AddUserCommand(newUser)));
            }
            catch (InvalidCredentialsException)
            {
                return Results.BadRequest("Invalid credentials format.");
            }
            catch (UserAlreadyExistsException)
            {
                return Results.BadRequest("User already exists.");
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        app.MapPost("/users/updateuser", async (
            [FromBody] UserDto user, 
            [FromServices] ISender sender) =>
        {
            try
            {
                await sender.Send(new UpdateUserCommand(user));
                return Results.Ok();
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });
        
        app.MapDelete("/users/removeuser", async (
            [FromBody] UserDto user, 
            [FromServices] ISender sender) =>
        {
            try
            {
                await sender.Send(new RemoveUserCommand(user));

                return Results.Ok();
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        app.MapPost("/users/username/exists", async (
            [FromBody] UserDto user, 
            [FromServices] ISender sender) =>
        {
            try
            {
                var usernameExists = await sender.Send(new CheckUsernameExistsQuery(user));
                return Results.Ok(usernameExists);
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        app.MapPost("/users/email/exists", async (
            [FromBody] UserDto user, 
            [FromServices] ISender sender) =>
        {
            try
            {
                var emailExists = await sender.Send(new CheckEmailExistsQuery(user));
                return Results.Ok(emailExists);
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });
    }
}
