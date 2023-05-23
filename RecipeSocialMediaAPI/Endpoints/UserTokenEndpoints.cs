using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.UserTokens.Commands;
using RecipeSocialMediaAPI.Handlers.UserTokens.Notifications;
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

            app.MapPost("/tokens/valid", ([FromHeader(Name = "authorizationToken")] string token, IUserValidationService validationService, IUserTokenService userTokenService) =>
            {
                return Results.Ok(userTokenService.CheckValidToken(token));
            });

            app.MapGet("/tokens/logout", async ([FromHeader(Name = "authorizationToken")] string token, IPublisher publisher, IUserTokenService userTokenService) =>
            {
                if (!userTokenService.CheckValidToken(token))
                {
                    return Results.Unauthorized();
                }

                await publisher.Publish(new RemoveTokenNotification(token));
                
                return Results.Ok();
            });
        }
    }
}
