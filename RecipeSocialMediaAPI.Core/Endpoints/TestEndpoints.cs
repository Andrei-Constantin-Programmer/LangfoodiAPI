using RecipeSocialMediaAPI.Application.Identity;

namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class TestEndpoints
{
    public static WebApplication MapTestEndpoints(this WebApplication app)
    {
        app.MapGroup("/test")
            .AddTestEndpoints()
            .WithTags("Test");

        return app;
    }

    private static RouteGroupBuilder AddTestEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/log", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Hello World");
        })
            .RequireAuthorization(IdentityData.DeveloperUserPolicyName);

        return group;
    }
}
