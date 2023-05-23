using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Querries;
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
            app.MapPost("/tokens/login", async (UserDto user, ISender sender, IPublisher publisher, IUserValidationService validationService, IUserTokenService userTokenService) =>
            {
                if (!await sender.Send(new ValidUserLoginQuery(user)))
                {
                    return Results.BadRequest("Invalid credentials");
                }

                if (!userTokenService.CheckTokenExists(user))
                {
                    return Results.Ok(await sender.Send(new GenerateTokenCommand(user)));
                }

                if (userTokenService.CheckTokenExpired(user))
                {
                    await publisher.Publish(new RemoveTokenForUserNotification(user));
                    return Results.Ok(await sender.Send(new GenerateTokenCommand(user)));
                }

                return Results.Ok(await sender.Send(new GetUserTokenQuery(user)));
            });

            app.MapPost("/tokens/valid", ([FromHeader(Name = "authorizationToken")] string token, IUserValidationService validationService, IUserTokenService userTokenService) =>
            {
                if (!userTokenService.CheckValidToken(token))
                {
                    return Results.BadRequest("Invalid/Expired token");
                }

                return Results.Ok();
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
