using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Images.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class ImageEndpoints
{
    public static WebApplication MapImageEndpoints(this WebApplication app)
    {
        app.MapGroup("/image")
            .AddImageEndpoints()
            .WithTags("Images");

        return app;
    }

    private static RouteGroupBuilder AddImageEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get/cloudinary-signature", async (
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetCloudinarySignatureQuery()));
        });

        group.MapDelete("/single-delete", async (
            [FromQuery] string publicId,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveImageCommand(publicId));
            return Results.Ok();
        });

        group.MapDelete("/bulk-delete", async (
            [FromQuery] string[] publicIds,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveMultipleImagesCommand(publicIds.ToList()));
            return Results.Ok();
        });

        return group;
    }
}
