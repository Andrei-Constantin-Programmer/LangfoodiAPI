using RecipeSocialMediaAPI.Presentation.Endpoints;

namespace RecipeSocialMediaAPI.Presentation.Configuration;

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
