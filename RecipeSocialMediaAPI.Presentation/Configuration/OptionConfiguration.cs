using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Presentation.Options;
using RecipeSocialMediaAPI.Presentation.OptionValidation;
using RecipeSocialMediaAPI.DataAccess.Helpers;

namespace RecipeSocialMediaAPI.Presentation.Configuration;

internal static class OptionConfiguration
{
    internal static void ConfigureOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<MongoDatabaseOptions>()
            .BindConfiguration(MongoDatabaseOptions.CONFIGURATION_SECTION)
            .ValidateOptions()
            .ValidateOnStart();
        
        builder.Services.AddOptions<CloudinaryApiOptions>()
            .BindConfiguration(CloudinaryApiOptions.CONFIGURATION_SECTION)
            .ValidateOptions()
            .ValidateOnStart();

        builder.Services.AddOptions<CloudinaryEndpointOptions>()
            .BindConfiguration(CloudinaryEndpointOptions.CONFIGURATION_SECTION)
            .ValidateOptions()
            .ValidateOnStart();

        builder.Services.AddOptions<DataDogOptions>()
            .BindConfiguration(DataDogOptions.CONFIGURATION_SECTION)
            .ValidateOptions()
            .ValidateOnStart();

        builder.Services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.CONFIGURATION_SECTION)
            .ValidateOptions()
            .ValidateOnStart();
    }
}
