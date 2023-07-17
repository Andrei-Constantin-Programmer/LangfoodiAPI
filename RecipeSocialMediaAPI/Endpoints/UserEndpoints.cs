using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/user/create", async (
            [FromBody] NewUserDTO newUser,
            [FromServices] ISender sender) =>
        {
            try
            {
                UserDTO user = await sender.Send(new AddUserCommand(newUser));

                return Results.Ok(user);
            }
            catch (ValidationException)
            {
                return Results.BadRequest("Invalid input.");
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

        app.MapPost("/user/update", async (
            [FromBody] UserDTO user, 
            [FromServices] ISender sender) =>
        {
            try
            {
                await sender.Send(new UpdateUserCommand(user));
                return Results.Ok();
            }
            catch (UserNotFoundException)
            {
                return Results.BadRequest("User not found.");
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });
        
        app.MapDelete("/user/remove", async (
            [FromQuery] string emailOrId, 
            [FromServices] ISender sender) =>
        {
            try
            {
                await sender.Send(new RemoveUserCommand(emailOrId));

                return Results.Ok();
            }
            catch (UserNotFoundException)
            {
                return Results.BadRequest("User not found.");
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        app.MapPost("/user/username/exists", async (
            [FromQuery] string username, 
            [FromServices] ISender sender) =>
        {
            try
            {
                return Results.Ok(await sender.Send(new CheckUsernameExistsQuery(username)));
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });

        app.MapPost("/user/email/exists", async (
            [FromQuery] string email, 
            [FromServices] ISender sender) =>
        {
            try
            {
                return Results.Ok(await sender.Send(new CheckEmailExistsQuery(email)));
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });
    }

    
}
