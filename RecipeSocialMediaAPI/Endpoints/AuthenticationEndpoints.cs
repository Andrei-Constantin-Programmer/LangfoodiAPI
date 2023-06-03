using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Authentication.Querries;
using RecipeSocialMediaAPI.Handlers.Users.Commands;

namespace RecipeSocialMediaAPI.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/authenticate", async (
            [FromBody] UserDto user,
            [FromServices] ISender sender) =>
        {
            try
            {
                var successfulLogin = await sender.Send(new AuthenticateUserQuery(user));
                return Results.Ok(successfulLogin);
            }
            catch (InvalidCredentialsException)
            {
                return Results.BadRequest("Invalid format.");
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });
    }
}
