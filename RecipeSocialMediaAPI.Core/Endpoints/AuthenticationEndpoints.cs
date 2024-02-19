using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeSocialMediaAPI.Application.Contracts.Authentication;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;
using RecipeSocialMediaAPI.Core.Options;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeSocialMediaAPI.Core.Endpoints;

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
            [FromServices] ISender sender,
            [FromServices] IOptions<JwtOptions> jwtOptions,
            [FromServices] IDateTimeProvider dateTimeProvider) =>
        {
            var user = await sender.Send(new AuthenticateUserQuery(authenticationAttempt.HandlerOrEmail, authenticationAttempt.Password));

            var jwtSettings = jwtOptions.Value;
            JwtSecurityTokenHandler tokenHandler = new();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, user.Id),
                }),
                Expires = dateTimeProvider.Now.Add(jwtSettings.Lifetime).UtcDateTime,
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Results.Ok(new SuccessfulAuthenticationDTO(user, tokenString));
        });

        return group;
    }
}
