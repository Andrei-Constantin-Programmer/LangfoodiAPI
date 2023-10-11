using RecipeSocialMediaAPI.Core.Endpoints;

namespace RecipeSocialMediaAPI.Core.Configuration;

internal static class EndpointsConfiguration
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapUserEndpoints();
        app.MapRecipeEndpoints();
        app.MapAuthenticationEndpoints();
        app.MapTestEndpoints();
    }
}
