using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Images.Queries;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication app)
    {
        app.MapGroup("/image")
            .AddImageEndpoints()
            .WithTags("Images");
    }

    private static RouteGroupBuilder AddImageEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/get/cloudinary-signature", async (
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetCloudinarySignatureQuery()));
        });

        group.MapPost("/single-delete", async (
            [FromBody] string publicId,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveImageCommand(publicId));
            return Results.Ok();
        });

        group.MapPost("/bulk-delete", async (
            [FromBody] List<string> publicIds,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveImagesCommand(publicIds));
            return Results.Ok();
        });

        return group;
    }
}
