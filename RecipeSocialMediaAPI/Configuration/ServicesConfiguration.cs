using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Mapper.Profiles;
using RecipeSocialMediaAPI.Services.Interfaces;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;

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
        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();
        builder.Services.AddSingleton<IUserValidationService, UserValidationService>();

        // Transients
        builder.Services.AddTransient<IUserService, UserService>();

        // Scoped

        // MediatR
        builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
    }
}
