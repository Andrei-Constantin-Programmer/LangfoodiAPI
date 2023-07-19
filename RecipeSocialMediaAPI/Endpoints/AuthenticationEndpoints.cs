using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.Handlers.Authentication.Querries;

namespace RecipeSocialMediaAPI.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/authenticate", async (
            [FromBody] AuthenticationAttemptContract authenticationAttempt,
            [FromServices] ISender sender) =>
        {
            var successfulLogin = await sender.Send(new AuthenticateUserQuery(authenticationAttempt.UsernameOrEmail, authenticationAttempt.Password));
            return Results.Ok(successfulLogin);
        });
    }
}
