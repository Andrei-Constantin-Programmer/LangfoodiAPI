using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Authentication;
using RecipeSocialMediaAPI.Application.Handlers.Authentication.Querries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapGroup("/auth")
            .AddAuthenticationEndpoints()
            .WithTags("Authentication");
    }

    private static RouteGroupBuilder AddAuthenticationEndpoints(this RouteGroupBuilder group)
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
}
