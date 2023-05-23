namespace RecipeSocialMediaAPI.Endpoints
{
    public static class TestEndpoints
    {
        public static void MapTestEndpoints(this WebApplication app)
        {
            app.MapPost("/logtest", (ILogger<Program> logger) =>
            {
                logger.LogInformation("Hello World");
            });
        }
    }
}
