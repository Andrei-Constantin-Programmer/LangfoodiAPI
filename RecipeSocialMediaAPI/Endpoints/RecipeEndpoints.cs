using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Handlers.Recipes.Queries;

namespace RecipeSocialMediaAPI.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this WebApplication app)
    {
        app.MapGet("/recipe/get", async (
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipesQuery()));
        });

        app.MapPost("/recipe/getById/{id}", async (
            [FromRoute] int id, 
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipeByIdQuery(id)));
        });

        app.MapPost("/recipe/create", async (
            [FromBody] RecipeDTO recipe,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new CreateRecipeCommand(recipe));
            return Results.Ok();
        });
    }
}
