using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Querries;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class UserTokenEndpoints
    {
        public static void MapUserTokenEndpoints(this WebApplication app)
        {
            app.MapPost("/tokens/login", async (UserDto user, ISender sender, IUserValidationService validationService, IUserTokenService userTokenService) =>
            {
                if (!await sender.Send(new ValidUserLoginQuery(user)))
                {
                    return Results.BadRequest("Invalid credentials");
                }

                if (!userTokenService.CheckTokenExists(user))
                {
                    return Results.Ok(userTokenService.GenerateToken(user));
                }

                if (userTokenService.CheckTokenExpired(user))
                {
                    userTokenService.RemoveToken(user);

                    return Results.Ok(userTokenService.GenerateToken(user));
                }

                return Results.Ok(userTokenService.GetTokenFromUser(user));
            });

            app.MapPost("/tokens/valid", ([FromHeader(Name = "authorization")] string token, IUserValidationService validationService, IUserTokenService userTokenService) =>
            {
                if (!userTokenService.CheckValidToken(token))
                {
                    return Results.BadRequest("Invalid/Expired token");
                }

                return Results.Ok(true);
            });

            app.MapGet("/tokens/logout", ([FromHeader(Name = "authorization")] string token, IUserTokenService userTokenService) =>
            {
                if (!userTokenService.CheckValidToken(token))
                {
                    return Results.Unauthorized();
                }
                if (!userTokenService.RemoveToken(token))
                {
                    return Results.BadRequest("Issue removing token");
                }
                
                return Results.Ok(true);
            });
        }
    }
}
