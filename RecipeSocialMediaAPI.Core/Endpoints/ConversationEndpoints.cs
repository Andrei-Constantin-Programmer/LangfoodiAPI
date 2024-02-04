using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class ConversationEndpoints
{
    public static WebApplication MapConversationEndpoints(this WebApplication app)
    {
        app.MapGroup("/conversation")
            .AddConversationEndpoints()
            .WithTags("Conversation");

        return app;
    }

    private static RouteGroupBuilder AddConversationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get-by-connection", async (
            [FromQuery] string connectionId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByConnectionQuery(connectionId)));
        });

        group.MapPost("/get-by-group", async (
            [FromQuery] string groupId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetConversationByGroupQuery(groupId)));
        });

        group.MapPost("/create-by-conversation", async (
        [FromBody] NewConversationContract newConversationContract,
        [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateConnectionConversationCommand(newConversationContract)));
        });

        group.MapPost("/create-by-group", async (
        [FromBody] NewConversationContract newConversationContract,
        [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new CreateGroupConversationCommand(newConversationContract)));
        });

        return group;
    }
}
