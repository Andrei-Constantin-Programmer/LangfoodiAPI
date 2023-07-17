using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.Authentication.Querries;
using RecipeSocialMediaAPI.Handlers.Users.Commands;

namespace RecipeSocialMediaAPI.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/authenticate", async (
            [FromBody] AuthenticationAttemptContract authenticationAttempt,
            [FromServices] ISender sender) =>
        {
            try
            {
                var successfulLogin = await sender.Send(new AuthenticateUserQuery(authenticationAttempt.UsernameOrEmail, authenticationAttempt.Password));
                return Results.Ok(successfulLogin);
            }
            catch (UserNotFoundException)
            {
                return Results.BadRequest("User not found.");
            }
            catch (InvalidCredentialsException)
            {
                return Results.BadRequest("Invalid credentials.");
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        });
    }
}
