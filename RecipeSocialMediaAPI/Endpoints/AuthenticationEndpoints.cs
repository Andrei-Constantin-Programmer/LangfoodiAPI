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
            [FromHeader] string usernameOrEmail,
            [FromHeader] string password,
            [FromServices] ISender sender) =>
        {
            try
            {
                var successfulLogin = await sender.Send(new AuthenticateUserQuery(usernameOrEmail, password));
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
