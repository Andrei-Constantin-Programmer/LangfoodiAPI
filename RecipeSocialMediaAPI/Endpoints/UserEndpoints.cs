using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Core.Handlers.Users.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/user/create", async (
            [FromBody] NewUserContract newUserContract,
            [FromServices] ISender sender) =>
        {
            UserDTO user = await sender.Send(new AddUserCommand(newUserContract));
            return Results.Ok(user);
        });

        app.MapPost("/user/update", async (
            [FromBody] UpdateUserContract updateUserContract, 
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateUserCommand(updateUserContract));
            return Results.Ok();
        });
        
        app.MapDelete("/user/remove", async (
            [FromQuery] string emailOrId, 
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveUserCommand(emailOrId));
            return Results.Ok();
        });

        app.MapPost("/user/username/exists", async (
            [FromQuery] string username, 
            [FromServices] ISender sender) =>
        {    
            return Results.Ok(await sender.Send(new CheckUsernameExistsQuery(username)));
        });

        app.MapPost("/user/email/exists", async (
            [FromQuery] string email, 
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CheckEmailExistsQuery(email)));  
        });
    }

    
}
