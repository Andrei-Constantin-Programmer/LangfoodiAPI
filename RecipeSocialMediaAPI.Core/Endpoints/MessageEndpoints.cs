using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        return group;
    }
}
