using Microsoft.Extensions.Options;

namespace RecipeSocialMediaAPI.Core.OptionValidation;

public static class OptionsBuilderExtensions
{
    public static OptionsBuilder<TOptions> ValidateOptions<TOptions>(this OptionsBuilder<TOptions> builder) where TOptions : class
    {
        builder.Services.AddSingleton<IValidateOptions<TOptions>>(
            serviceProvider => new ValidateOptions<TOptions>(
                serviceProvider,
                builder.Name));

        return builder;
    }
}
