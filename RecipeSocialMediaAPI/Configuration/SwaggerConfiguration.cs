using Microsoft.OpenApi.Models;

namespace RecipeSocialMediaAPI.Core.Configuration;

internal static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "authorization",
                Description = "Authorization string expects user token",
                Scheme = "ApiKeyScheme"
            });

            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };

            var requirement = new OpenApiSecurityRequirement
            {
                { key, new List<string>() }
            };
            options.AddSecurityRequirement(requirement);

        });
    }
}
