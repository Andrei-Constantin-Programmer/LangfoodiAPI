using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapPost("/users/createuser", async (UserDto newUser, ISender sender) =>
            {
                try
                {
                    var token = await sender.Send(new AddUserCommand(newUser));
                    return Results.Ok(token);
                }
                catch (InvalidCredentialsException)
                {
                    return Results.BadRequest("Invalid credentials format.");
                }
                catch (Exception)
                {
                    return Results.StatusCode(500);
                }
            });

            app.MapPost("/users/updateuser", async ([FromHeader(Name = "authorizationToken")] string token, UserDto user, ISender sender) =>
            {
                try
                {
                    await sender.Send(new UpdateUserCommand(user, token));
                    return Results.Ok();
                }
                catch (TokenNotFoundOrExpiredException)
                {
                    return Results.Unauthorized();
                }
                catch (Exception)
                {
                    return Results.StatusCode(500);
                }
            });
            
            app.MapDelete("/users/removeuser", async ([FromHeader(Name = "authorizationToken")] string token, ISender sender) =>
            {
                try
                {
                    await sender.Send(new RemoveUserCommand(token));

                    return Results.Ok();
                }
                catch (TokenNotFoundOrExpiredException)
                {
                    return Results.Unauthorized();
                }
                catch (Exception)
                {
                    return Results.StatusCode(500);
                }
            });

            app.MapPost("/users/username/exists", async (UserDto user, ISender sender) =>
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

            app.MapPost("/users/email/exists", async (UserDto user, ISender sender) =>
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
}
