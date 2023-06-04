﻿using MediatR;
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
        app.MapPost("/users/createuser", async (
            [FromBody] UserDto newUser, 
            [FromServices] ISender sender) =>
        {
            try
            {
                return Results.Ok(await sender.Send(new AddUserCommand(newUser)));
            }
            catch (Exception ex) when (ex 
                is ArgumentException 
                or InvalidCredentialsException)
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

        app.MapPost("/users/username/exists", async (
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

        app.MapPost("/users/email/exists", async (
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
