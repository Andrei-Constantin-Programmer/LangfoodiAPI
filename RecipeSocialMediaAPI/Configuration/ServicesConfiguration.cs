using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using RecipeSocialMediaAPI.Cryptography;
using RecipeSocialMediaAPI.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Mappers.Profiles;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Services.Interfaces;
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
        builder.Services.AddSingleton(GenerateDatabaseConfiguration(builder.Configuration));
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();
        builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
        builder.Services.AddSingleton<IMongoCollectionFactory, MongoCollectionFactory>();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

        // Transients
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IUserDocumentToModelMapper, UserDocumentToModelMapper>();

        // Scoped
        builder.Services.AddScoped<ICryptoService, CryptoService>();

        // MediatR
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<Program>();
            config.AddOpenRequestPreProcessor(typeof(ValidationPreProcessor<>));
        });
    }

    private static DatabaseConfiguration GenerateDatabaseConfiguration(ConfigurationManager configurationManager) => new(
        configurationManager.GetSection("MongoDB").GetValue<string>("Connection") ?? string.Empty,
        configurationManager.GetSection("MongoDB").GetValue<string>("ClusterName") ?? string.Empty);
}
