using RecipeSocialMediaAPI.Core.OptionValidation;
using RecipeSocialMediaAPI.DataAccess.Helpers;

namespace RecipeSocialMediaAPI.Core.Configuration;

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
    }
}
