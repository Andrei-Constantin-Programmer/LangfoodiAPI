using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Recipes.Queries;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

public static class RecipeEndpoints
{
    public static WebApplication MapRecipeEndpoints(this WebApplication app)
    {
        app.MapGroup("/recipe")
            .AddRecipeEndpoints()
            .WithTags("Recipe");

        return app;
    }

    private static RouteGroupBuilder AddRecipeEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get/id", async (
            [FromQuery] string id,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipeByIdQuery(id), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets a recipe by its id.")
            .Produces<RecipeDetailedDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get/userid", async (
            [FromQuery] string id,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipesFromUserIdQuery(id), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all recipes created by user.")
            .Produces<List<RecipeDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/get/username", async (
            [FromQuery] string username,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetRecipesFromUserQuery(username), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Gets all recipes created by user.")
            .Produces<List<RecipeDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/create", async (
            [FromBody] NewRecipeContract newRecipeContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new AddRecipeCommand(newRecipeContract), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Creates a new recipe.")
            .Produces<RecipeDetailedDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/update", async (
            [FromBody] UpdateRecipeContract updateRecipeContract,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new UpdateRecipeCommand(updateRecipeContract), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Updates a recipe.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/remove", async (
            [FromQuery] string id,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveRecipeCommand(id), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Deletes a recipe.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
