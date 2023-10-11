namespace RecipeSocialMediaAPI.Core.Endpoints;

public static class TestEndpoints
{
    public static void MapTestEndpoints(this WebApplication app)
    {
        app.MapGroup("/test")
            .AddTestEndpoints()
            .WithTags("Test");
    }

    private static RouteGroupBuilder AddTestEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/log", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Hello World");
        });

        return group;
    }
}
