using RecipeSocialMediaAPI.Application.Identity;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Presentation.Endpoints;

public static class TestEndpoints
{
    public static WebApplication MapTestEndpoints(this WebApplication app)
    {
        app.MapGroup("/test")
            .AddTestEndpoints()
            .WithTags("Test");

        return app;
    }

    [ExcludeFromCodeCoverage(Justification = "No testing necessary for test endpoint")]
    private static RouteGroupBuilder AddTestEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/log", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Hello World");
        })
            .RequireAuthorization(IdentityData.DeveloperUserPolicyName)
            .WithDescription("Creates a test log.")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        return group;
    }
}
