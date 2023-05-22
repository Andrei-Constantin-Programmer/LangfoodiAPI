using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Handlers.Users.Querries;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapPost("/users/createuser", async (ISender sender, IUserTokenService userTokenService, IUserValidationService validationService, IUserService userService, UserDto newUser) =>
            {
                if (!await validationService.ValidUserAsync(newUser, userService))
                {
                    return Results.BadRequest("Invalid credentials format");
                }

                return Results.Ok(sender.Send(new AddUserCommand(newUser)));
            });

            app.MapPost("/users/updateuser", ([FromHeader(Name = "authorizationToken")] string token, IUserValidationService validationService, IUserTokenService userTokenService, IUserService userService, UserDto user) =>
            {
                if (!userTokenService.CheckValidToken(token))
                {
                    return Results.Unauthorized();
                }
                if (!userService.UpdateUser(validationService, userTokenService, token, user))
                {
                    return Results.BadRequest("Issue updating user");
                }
                
                return Results.Ok(true);
            });
            
            app.MapDelete("/users/removeuser/", async ([FromHeader(Name = "authorizationToken")] string token, ISender sender, IUserTokenService userTokenService, IUserService userService) =>
            {
                if (!userTokenService.CheckValidToken(token)) 
                { 
                    return Results.Unauthorized(); 
                }
                if (!await sender.Send(new RemoveUserCommand(token)))
                {
                    return Results.BadRequest("Issue removing user");
                }

                return Results.Ok(true);
            });

            app.MapPost("/users/username/exists", async (ISender sender, IUserValidationService validationService, IUserService userService, UserDto user) =>
            {
                if (!validationService.ValidUserName(user.UserName))
                {
                    return Results.BadRequest("Invalid username format");
                }

                return Results.Ok(await sender.Send(new CheckUsernameExistsQuery(user)));
            });

            app.MapPost("/users/email/exists", async (ISender sender, IUserValidationService validationService, IUserService userService, UserDto user) =>
            {
                if (!validationService.ValidEmail(user.Email))
                {
                    return Results.BadRequest("Invalid email format");
                }

                return Results.Ok(await sender.Send(new CheckEmailExistsQuery(user)));
            });
        }
    }
}
