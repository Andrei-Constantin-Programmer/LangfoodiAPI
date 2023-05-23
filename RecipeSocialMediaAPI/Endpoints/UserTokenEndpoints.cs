using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.UserTokens.Commands;
using RecipeSocialMediaAPI.Handlers.UserTokens.Notifications;
using RecipeSocialMediaAPI.Handlers.UserTokens.Querries;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class UserTokenEndpoints
    {
        public static void MapUserTokenEndpoints(this WebApplication app)
        {
            app.MapPost("/tokens/login", async (UserDto user, ISender sender) =>
            {
                try
                {
                    var token = await sender.Send(new GetOrCreateUserTokenCommand(user));
                    return Results.Ok(token);
                }
                catch (UserNotFoundException)
                {
                    return Results.NotFound();
                }
                catch (Exception)
                {
                    return Results.StatusCode(500);
                }
            });

            app.MapPost("/tokens/valid", async ([FromHeader(Name = "authorizationToken")] string token, ISender sender) =>
            {
                try
                {
                    var existsAndIsNotExpired = await sender.Send(new GetIsValidUserTokenQuery(token));
                    return Results.Ok();
                }
                catch (Exception)
                {
                    return Results.StatusCode(500);
                }
            });

            app.MapGet("/tokens/logout", async ([FromHeader(Name = "authorizationToken")] string token, IPublisher publisher) =>
            {
                try
                {
                    await publisher.Publish(new RemoveTokenNotification(token));
                
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
        }
    }
}
