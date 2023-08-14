using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Core.DTO;
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
        group.MapGet("/get", async (
            [FromServices] ISender sender) =>
        {
            return Results.NotFound(/*await sender.Send(new GetRecipesQuery())*/);
        });

        group.MapPost("/getById/{id}", async (
            [FromRoute] int id,
            [FromServices] ISender sender) =>
        {
            return Results.NotFound(/*await sender.Send(new GetRecipeByIdQuery(id))*/);
        });

        group.MapPost("/create", async (
            [FromBody] RecipeDTO recipe,
            [FromServices] ISender sender) =>
        {
            //await sender.Send(new CreateRecipeCommand(recipe));
            return Results.NotFound();
        });

        return group;
    }
}
