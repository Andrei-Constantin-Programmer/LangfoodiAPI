using RecipeSocialMediaAPI.Endpoints;

namespace RecipeSocialMediaAPI.Configuration;

internal static class EndpointsConfiguration
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapUserEndpoints();
        app.MapRecipeEndpoints();
        app.MapTestEndpoints();
        app.MapAuthenticationEndpoints();
    }
}
