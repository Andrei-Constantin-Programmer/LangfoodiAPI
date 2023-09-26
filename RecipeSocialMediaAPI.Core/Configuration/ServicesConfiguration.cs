using FluentValidation;
using RecipeSocialMediaAPI.Application.Cryptography;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Users;
using RecipeSocialMediaAPI.Application.Utilities.Interfaces;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories;
using RecipeSocialMediaAPI.Application.Repositories;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Profiles;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Repositories.Users;

namespace RecipeSocialMediaAPI.Core.Configuration;

internal static class ServicesConfiguration
{
    internal static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // AutoMapper
        builder.Services.AddAutoMapper(typeof(UserMappingProfile));

        // Singletons
        builder.Services.AddSingleton(GenerateDatabaseConfiguration(builder.Configuration));
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
        builder.Services.AddSingleton<IRecipeValidationService, RecipeValidationService>();
        builder.Services.AddSingleton<IMongoCollectionFactory, MongoCollectionFactory>();
        builder.Services.AddSingleton<IUserDocumentToModelMapper, UserDocumentToModelMapper>();
        builder.Services.AddSingleton<IRecipeDocumentToModelMapper, RecipeDocumentToModelMapper>();
        builder.Services.AddSingleton<IRecipeMapper, RecipeMapper>();
        builder.Services.AddSingleton<IUserMapper, UserMapper>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
        builder.Services.AddValidatorsFromAssemblyContaining<IDateTimeProvider>(ServiceLifetime.Singleton);

        // Scoped
        builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
        builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
        builder.Services.AddScoped<IUserPersistenceRepository, UserPersistenceRepository>();

        // Transients
        builder.Services.AddTransient<ICryptoService, CryptoService>();

        // MediatR
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<Program>();
            config.RegisterServicesFromAssemblyContaining<IDateTimeProvider>();
            config.AddOpenRequestPreProcessor(typeof(ValidationPreProcessor<>));
        });
    }

    private static DatabaseConfiguration GenerateDatabaseConfiguration(ConfigurationManager configurationManager) => new(
        configurationManager.GetSection("MongoDB").GetValue<string>("Connection") ?? string.Empty,
        configurationManager.GetSection("MongoDB").GetValue<string>("ClusterName") ?? string.Empty);
}
