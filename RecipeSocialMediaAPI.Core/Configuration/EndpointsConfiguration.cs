using RecipeSocialMediaAPI.Core.Endpoints;

namespace RecipeSocialMediaAPI.Core.Configuration;

public static class EndpointsConfiguration
{
    public static void MapEndpoints(this WebApplication app)
    {
        app
            .MapAuthenticationEndpoints()
            .MapUserEndpoints()
            .MapRecipeEndpoints()
            .MapImageEndpoints()
            .MapMessageEndpoints()
            .MapConnectionEndpoints()
            .MapGroupEndpoints()
            .MapConversationEndpoints()
            .MapTestEndpoints();
    }
}
