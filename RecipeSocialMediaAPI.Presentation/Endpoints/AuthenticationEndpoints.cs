using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Authentication;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

public static class AuthenticationEndpoints
{
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapGroup("/auth")
            .AddAuthenticationEndpoints()
            .WithTags("Authentication");

        return app;
    }

    private static RouteGroupBuilder AddAuthenticationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/authenticate", async (
            [FromBody] AuthenticationAttemptContract authenticationAttempt,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            var successfulAuthentication = await sender
                .Send(new AuthenticateUserQuery(authenticationAttempt.Email, authenticationAttempt.Password), cancellationToken);

            return Results.Ok(successfulAuthentication);
        })
            .WithDescription("Authenticates user. If successful, returns the user and a bearer token.")
            .Produces<SuccessfulAuthenticationDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
