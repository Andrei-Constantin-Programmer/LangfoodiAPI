using RecipeSocialMediaAPI.Core.Endpoints;

namespace RecipeSocialMediaAPI.Core.Configuration;

internal static class EndpointsConfiguration
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGroup("/user")
            .UserEndpoints()
            .WithTags("User");

        app.MapGroup("/recipe")
            .RecipeEndpoints()
            .WithTags("Recipe");

        app.MapGroup("/test")
            .TestEndpoints()
            .WithTags("Test");

        app.MapGroup("/auth")
            .AuthenticationEndpoints()
            .WithTags("Authentication");
    }
}
