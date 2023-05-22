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
            app.MapPost("/users/createuser", async (UserDto newUser, ISender sender, IUserTokenService userTokenService, IUserValidationService validationService) =>
            {
                if (!await validationService.ValidUserAsync(newUser))
                {
                    return Results.BadRequest("Invalid credentials format");
                }

                return Results.Ok(sender.Send(new AddUserCommand(newUser)));
            });

            app.MapPost("/users/updateuser", async ([FromHeader(Name = "authorizationToken")] string token, ISender sender, IUserValidationService validationService, IUserTokenService userTokenService, UserDto user) =>
            {
                if (!userTokenService.CheckValidToken(token))
                {
                    return Results.Unauthorized();
                }
                if (!await sender.Send(new UpdateUserCommand(user, token)))
                {
                    return Results.BadRequest("Issue updating user");
                }
                
                return Results.Ok();
            });
            
            app.MapDelete("/users/removeuser", async ([FromHeader(Name = "authorizationToken")] string token, ISender sender, IUserTokenService userTokenService) =>
            {
                if (!userTokenService.CheckValidToken(token)) 
                { 
                    return Results.Unauthorized(); 
                }
                if (!await sender.Send(new RemoveUserCommand(token)))
                {
                    return Results.BadRequest("Issue removing user");
                }

                return Results.Ok();
            });

            app.MapPost("/users/username/exists", async (UserDto user, ISender sender, IUserValidationService validationService) =>
            {
                if (!validationService.ValidUserName(user.UserName))
                {
                    return Results.BadRequest("Invalid username format");
                }

                return Results.Ok(await sender.Send(new CheckUsernameExistsQuery(user)));
            });

            app.MapPost("/users/email/exists", async (UserDto user, ISender sender, IUserValidationService validationService) =>
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
