using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        return group;
    }
}
