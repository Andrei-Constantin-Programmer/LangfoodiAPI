using RecipeSocialMediaAPI.Core.Endpoints;

namespace RecipeSocialMediaAPI.Core.Configuration;

public static class EndpointsConfiguration
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapUserEndpoints();
        app.MapRecipeEndpoints();
        app.MapAuthenticationEndpoints();
        app.MapImageEndpoints();
        app.MapTestEndpoints();
        app.MapConnectionEndpoints();
        app.MapMessageEndpoints();
    }
}
