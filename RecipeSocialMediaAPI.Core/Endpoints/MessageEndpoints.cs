using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Contracts.Recipes;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        app.MapGroup("/message")
            .AddMessageEndpoints()
            .WithTags("Message");
    }

    private static RouteGroupBuilder AddMessageEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get", async (
            [FromQuery] string id,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetMessageByIdQuery(id)));
        });

        return group;
    }
}
