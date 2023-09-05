using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Core.Handlers.Recipes.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this WebApplication app)
    {
        app.MapGroup("/recipe")
            .AddRecipeEndpoints()
            .WithTags("Recipe");
    }

    private static RouteGroupBuilder AddRecipeEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/getById", async (
            [FromQuery] string id,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipeByIdQuery(id)));
        });

        group.MapPost("/getRecipesFromUserId", async (
            [FromQuery] string id,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipesFromUserIdQuery(id)));
        });

        group.MapPost("/getRecipesFromUser", async (
            [FromQuery] string username,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipesFromUserQuery(username)));
        });

        group.MapPost("/create", async (
            [FromBody] NewRecipeContract newRecipeContract,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new AddRecipeCommand(newRecipeContract)));
        });

        group.MapPost("/update", async (
            [FromBody] UpdateRecipeContract updateRecipeContract,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateRecipeCommand(updateRecipeContract));
            return Results.Ok();
        });

        group.MapDelete("/remove", async (
            [FromQuery] string id,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveRecipeCommand(id));
            return Results.Ok();
        });

        return group;
    }
}
