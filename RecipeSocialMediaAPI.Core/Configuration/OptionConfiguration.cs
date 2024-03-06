using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Core.Options;
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
        
        builder.Services.AddOptions<CloudinaryOptions>()
            .BindConfiguration(CloudinaryOptions.CONFIGURATION_SECTION)
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
