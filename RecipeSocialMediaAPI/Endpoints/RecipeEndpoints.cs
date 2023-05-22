using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Handlers.Recipes.Querries;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class RecipeEndpoints
    {
        public static void MapRecipeEndpoints(this WebApplication app)
        {
            app.MapGet("/recipes/get", async (
                [FromServices] ISender sender) =>
            {
                return Results.Ok(await sender.Send(new GetRecipesQuery()));
            });

            app.MapPost("/recipes/getById/{id}", async (
                [FromRoute] int id, 
                [FromServices] ISender sender) =>
            {
                try
                {
                    return Results.Ok(await sender.Send(new GetRecipeByIdQuery(id)));
                }
                catch (RecipeNotFoundException)
                {
                    return Results.NotFound();
                }
            });

            app.MapPost("/recipes/create", async (
                [FromBody] RecipeDTO recipe,
                [FromServices] ISender sender) =>
            {
                await sender.Send(new CreateRecipeCommand(recipe));

                return Results.Ok();
            });
        }
    }
}
