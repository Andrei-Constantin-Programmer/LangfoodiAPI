using FluentValidation;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Mapper.Profiles;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;
using RecipeSocialMediaAPI.Validation;

namespace RecipeSocialMediaAPI.Configuration;

internal static class ServicesConfiguration
{
    internal static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // AutoMapper
        builder.Services.AddAutoMapper(typeof(UserMappingProfile));

        // Singletons
        builder.Services.AddSingleton<IMongoCollectionFactory, MongoCollectionFactory>();
        builder.Services.AddSingleton<IConfigManager, ConfigManager>();
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();
        builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

        // Transients
        builder.Services.AddTransient<IUserService, UserService>();

        // Scoped

        // MediatR
        builder.Services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<Program>()
        );
    }
}
