using MediatR;
using Microsoft.AspNetCore.Mvc;
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
            [FromQuery] string? publicId,
            [FromServices] ISender sender) =>
        {
            return Results.Ok(await sender.Send(new GetCloudinarySignatureQuery(publicId)));
        });

        return group;
    }
}
