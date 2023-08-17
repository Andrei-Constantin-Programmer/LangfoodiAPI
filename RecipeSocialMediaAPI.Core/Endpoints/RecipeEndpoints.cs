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
        group.MapPost("/getById/{id}", async (
            [FromRoute] string id,
            [FromServices] ISender sender) =>
        {
            return Results.NotFound(/*await sender.Send(new GetRecipeByIdQuery(id))*/);
        });

        group.MapPost("/getRecipesFromUserId/{id}", async (
            [FromRoute] string id,
            [FromServices] ISender sender) =>
        {
            return Results.NotFound(/*await sender.Send(new GetRecipesFromUserIdQuery(id))*/);
        });

        group.MapPost("/getRecipesFromUser", async (
            [FromQuery] string username,
            [FromServices] ISender sender) =>
        {
            return Results.NotFound(/*await sender.Send(new GetRecipesFromUserQuery(username))*/);
        });

        group.MapPost("/create", async (
            [FromBody] NewRecipeContract newRecipeContract,
            [FromServices] ISender sender) =>
        {
            //await sender.Send(new AddRecipeCommand(recipe));
            return Results.NotFound();
        });

        group.MapPost("/update", async (
            [FromBody] UpdateRecipeContract updateRecipeContract,
            [FromServices] ISender sender) =>
        {
            //await sender.Send(new CreateRecipeCommand(recipe));
            return Results.NotFound();
        });

        return group;
    }
}
