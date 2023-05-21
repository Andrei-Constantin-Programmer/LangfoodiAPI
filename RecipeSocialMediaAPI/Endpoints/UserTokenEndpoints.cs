using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class UserTokenEndpoints
    {
        public static void MapUserTokenEndpoints(this WebApplication app)
        {
            app.MapPost("/tokens/login", (IValidationService validationService, IUserService userService, IUserTokenService userTokenService, UserDto user) =>
            {
                if (!userService.ValidUserLogin(validationService, user)) return Results.BadRequest("Invalid credentials");
                if (!userTokenService.CheckTokenExists(user)) return Results.Ok(userTokenService.GenerateToken(user));
                if (userTokenService.CheckTokenExpired(user))
                {
                    userTokenService.RemoveToken(user);
                    return Results.Ok(userTokenService.GenerateToken(user));
                }

                return Results.Ok(userTokenService.GetTokenFromUser(user));
            });

            app.MapPost("/tokens/exists", ([FromHeader(Name = "authorization")] string token, IValidationService validationService, IUserTokenService userTokenService) =>
            {
                if (!userTokenService.CheckValidToken(token)) return Results.BadRequest("Invalid/Expired token");
                return Results.Ok(true);
            });

            app.MapGet("/tokens/logout", ([FromHeader(Name = "authorization")] string token, IUserTokenService userTokenService) =>
            {
                if (!userTokenService.CheckValidToken(token)) return Results.Unauthorized();
                if (!userTokenService.RemoveToken(token)) return Results.BadRequest("Issue removing token");
                
                return Results.Ok(true);
            });
        }
    }
}
