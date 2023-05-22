using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapPost("/users/createuser", (IUserTokenService userTokenService, IValidationService validationService, IUserService userService, UserDto newUser) =>
            {
                if (!validationService.ValidUser(newUser, userService)) return Results.BadRequest("Invalid credentials format");
                return Results.Ok(userService.AddUser(newUser, userTokenService, validationService));
            });

            app.MapPost("/users/updateuser", ([FromHeader(Name = "authorization")] string token, IValidationService validationService, IUserTokenService userTokenService, IUserService userService, UserDto user) =>
            {
                if (!userTokenService.CheckValidToken(token)) return Results.Unauthorized();
                if (!userService.UpdateUser(validationService, userTokenService, token, user)) return Results.BadRequest("Issue updating user");
                
                return Results.Ok(true);
            });

            app.MapDelete("/users/removeuser/", ([FromHeader(Name = "authorization")] string token, IUserTokenService userTokenService, IUserService userService) =>
            {
                if (!userTokenService.CheckValidToken(token)) return Results.Unauthorized();
                if (!userService.RemoveUser(token, userTokenService)) return Results.BadRequest("Issue removing user");

                return Results.Ok(true);
            });

            app.MapPost("/users/username/exists", (IValidationService validationService, IUserService userService, UserDto user) =>
            {
                if (!validationService.ValidUserName(user.UserName)) return Results.BadRequest("Invalid username format");
                return Results.Ok(userService.CheckUserNameExists(user));
            });

            app.MapPost("/users/email/exists", (IValidationService validationService, IUserService userService, UserDto user) =>
            {
                if (!validationService.ValidEmail(user.Email)) return Results.BadRequest("Invalid email format");
                return Results.Ok(userService.CheckEmailExists(user));
            });
        }
    }
}
