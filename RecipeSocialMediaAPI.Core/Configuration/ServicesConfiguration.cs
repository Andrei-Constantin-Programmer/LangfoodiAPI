using FluentValidation;
using RecipeSocialMediaAPI.Core.Cryptography;
using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Mappers.Profiles;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.Core.Mappers.Interfaces;
using RecipeSocialMediaAPI.Core.Mappers;
using RecipeSocialMediaAPI.Domain.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Mappers;

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
        builder.Services.AddSingleton<IMongoCollectionFactory, MongoCollectionFactory>();
        builder.Services.AddSingleton<IUserDocumentToModelMapper, UserDocumentToModelMapper>();
        builder.Services.AddSingleton<IRecipeDocumentToModelMapper, RecipeDocumentToModelMapper>();
        builder.Services.AddSingleton<IIngredientMapper, IngredientMapper>();
        builder.Services.AddSingleton<IRecipeStepMapper, RecipeStepMapper>();
        builder.Services.AddSingleton<IUserMapper, UserMapper>();
        builder.Services.AddSingleton<IRecipeContractToRecipeMapper, RecipeContractToRecipeMapper>();
        builder.Services.AddSingleton<IRecipeAggregateToRecipeDetailedDtoMapper, RecipeAggregateToRecipeDetailedDtoMapper>();
        builder.Services.AddSingleton<IRecipeAggregateToRecipeDtoMapper, RecipeAggregateToRecipeDtoMapper>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

        // Transients
        builder.Services.AddTransient<IRecipeRepository, RecipeRepository>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();

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
