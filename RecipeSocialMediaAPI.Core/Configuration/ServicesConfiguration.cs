﻿using FluentValidation;
using RecipeSocialMediaAPI.Application.Cryptography;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Repositories.Users;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.DataAccess.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;
using RecipeSocialMediaAPI.DataAccess.Repositories.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Messages;

namespace RecipeSocialMediaAPI.Core.Configuration;

internal static class ServicesConfiguration
{
    internal static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Singletons
        builder.Services.AddSingleton(GenerateCloudinaryConfiguration(builder.Configuration));
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
        builder.Services.AddSingleton<IRecipeValidationService, RecipeValidationService>();
        builder.Services.AddSingleton<IMongoCollectionFactory, MongoCollectionFactory>();
        builder.Services.AddSingleton<IRecipeMapper, RecipeMapper>();
        builder.Services.AddSingleton<IUserMapper, UserMapper>();
        builder.Services.AddSingleton<IMessageMapper, MessageMapper>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
        builder.Services.AddValidatorsFromAssemblyContaining<DateTimeProvider>(ServiceLifetime.Singleton);

        // Scoped
        builder.Services.AddScoped<IUserDocumentToModelMapper, UserDocumentToModelMapper>();
        builder.Services.AddScoped<IRecipeDocumentToModelMapper, RecipeDocumentToModelMapper>();
        builder.Services.AddScoped<IMessageDocumentToModelMapper, MessageDocumentToModelMapper>();
        builder.Services.AddScoped<IConnectionDocumentToModelMapper, ConnectionDocumentToModelMapper>();

        builder.Services.AddScoped<IRecipeQueryRepository, RecipeQueryRepository>();
        builder.Services.AddScoped<IRecipePersistenceRepository, RecipePersistenceRepository>();

        builder.Services.AddScoped<IMessageQueryRepository, MessageQueryRepository>();
        builder.Services.AddScoped<IMessagePersistenceRepository, MessagePersistenceRepository>();
        builder.Services.AddScoped<IConnectionQueryRepository, ConnectionQueryRepository>();
        builder.Services.AddScoped<IConnectionPersistenceRepository, ConnectionPersistenceRepository>();

        builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
        builder.Services.AddScoped<IUserPersistenceRepository, UserPersistenceRepository>();

        builder.Services.AddScoped<IImageHostingQueryRepository, ImageHostingQueryRepository>();

        // Transients
        builder.Services.AddTransient<ICryptoService, CryptoService>();
        builder.Services.AddTransient<IMessageFactory, MessageFactory>();
        builder.Services.AddTransient<IUserFactory, UserFactory>();

        // MediatR
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<Program>();
            config.RegisterServicesFromAssemblyContaining<DateTimeProvider>();
            config.AddOpenRequestPreProcessor(typeof(ValidationPreProcessor<>));
        });
    }

    private static CloudinaryApiConfiguration GenerateCloudinaryConfiguration(ConfigurationManager configurationManager) => new(
        configurationManager.GetSection("Cloudinary").GetValue<string>("CloudName") ?? string.Empty,
        configurationManager.GetSection("Cloudinary").GetValue<string>("ApiKey") ?? string.Empty,
        configurationManager.GetSection("Cloudinary").GetValue<string>("ApiSecret") ?? string.Empty);
}
