using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Infrastructure.Helpers;
using RecipeSocialMediaAPI.Presentation.Options;
using RecipeSocialMediaAPI.Presentation.OptionValidation;

namespace RecipeSocialMediaAPI.Presentation.Configuration;

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

        builder.Services.AddOptions<EncryptionOptions>()
            .BindConfiguration(EncryptionOptions.CONFIGURATION_SECTION)
            .ValidateOptions()
            .ValidateOnStart();
    }
}
