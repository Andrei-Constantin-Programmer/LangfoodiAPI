using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Handlers.Images.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Images.Queries;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

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
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetCloudinarySignatureQuery(), cancellationToken));
        })
            .RequireAuthorization()
            .WithDescription("Generates a Cloudinary signature.")
            .Produces<CloudinarySignatureDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/single-delete", async (
            [FromQuery] string publicId,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveImageCommand(publicId), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Deletes an image by its Cloudinary id.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/bulk-delete", async (
            [FromQuery] string[] publicIds,
            CancellationToken cancellationToken,
            [FromServices] ISender sender) =>
        {
            await sender.Send(new RemoveMultipleImagesCommand(publicIds.ToList()), cancellationToken);
            return Results.Ok();
        })
            .RequireAuthorization()
            .WithDescription("Deletes multiple images by their Cloudinary ids.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        return group;
    }
}
