using MediatR;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Mediator.Commands.Recipes;
using RecipeSocialMediaAPI.Mediator.Queries.Recipes;

namespace RecipeSocialMediaAPI.Endpoints
{
    public static class RecipeEndpoints
    {
        public static void MapRecipeEndpoints(this WebApplication app)
        {
            app.MapGet("/recipes/get", async (ISender sender) =>
            {
                return Results.Ok(await sender.Send(new GetRecipesQuery()));
            });

            app.MapPost("/recipes/create", async (RecipeDTO recipe, ISender sender) =>
            {
                await sender.Send(new AddRecipeCommand(recipe));

                return Results.Ok();
            });
        }
    }
}
