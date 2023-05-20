using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Mediator.Commands.Recipes;
using RecipeSocialMediaAPI.Mediator.Queries.Recipes;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class RecipeEndpoints
    {
        public static void MapRecipeEndpoints(this WebApplication app)
        {
            app.MapGet("/recipes/get", async ([FromServices] ISender sender) =>
            {
                return Results.Ok(await sender.Send(new GetRecipesQuery()));
            });

            app.MapPost("/recipes/create", async (RecipeDTO recipe, [FromServices] ISender sender) =>
            {
                await sender.Send(new AddRecipeCommand(recipe));

                return Results.Ok();
            });
        }
    }
}
