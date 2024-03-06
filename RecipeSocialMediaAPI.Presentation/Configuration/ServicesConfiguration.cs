using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Cryptography;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Messages;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Recipes;
using RecipeSocialMediaAPI.Application.Mappers.Recipes.Interfaces;
using RecipeSocialMediaAPI.Application.Mappers.Users;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Services;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Utilities;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Application.WebClients;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;
using RecipeSocialMediaAPI.Presentation.Middleware;
using RecipeSocialMediaAPI.Presentation.SignalR;
using RecipeSocialMediaAPI.Infrastructure.Mappers;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Messages;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Recipes;
using RecipeSocialMediaAPI.Infrastructure.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.Presentation;

namespace RecipeSocialMediaAPI.Core.Configuration;

internal static class ServicesConfiguration
{
    internal static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Singletons
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
        builder.Services.AddSingleton<IRecipeValidationService, RecipeValidationService>();
        builder.Services.AddSingleton<IRecipeMapper, RecipeMapper>();
        builder.Services.AddSingleton<IUserMapper, UserMapper>();
        builder.Services.AddSingleton<IMessageMapper, MessageMapper>();
        builder.Services.AddSingleton<IConversationMapper, ConversationMapper>();

        builder.Services.AddValidatorsFromAssemblyContaining<Application.AssemblyReference>(ServiceLifetime.Singleton);

        // Scoped
        builder.Services.AddValidatorsFromAssemblyContaining<AssemblyReference>();
        builder.Services.AddScoped<IMongoCollectionFactory, MongoCollectionFactory>();

        builder.Services.AddScoped<IUserDocumentToModelMapper, UserDocumentToModelMapper>();
        builder.Services.AddScoped<IRecipeDocumentToModelMapper, RecipeDocumentToModelMapper>();
        builder.Services.AddScoped<IMessageDocumentToModelMapper, MessageDocumentToModelMapper>();
        builder.Services.AddScoped<IConnectionDocumentToModelMapper, ConnectionDocumentToModelMapper>();
        builder.Services.AddScoped<IConversationDocumentToModelMapper, ConversationDocumentToModelMapper>();
        builder.Services.AddScoped<IGroupDocumentToModelMapper, GroupDocumentToModelMapper>();

        builder.Services.AddScoped<IRecipeQueryRepository, RecipeQueryRepository>();
        builder.Services.AddScoped<IRecipePersistenceRepository, RecipePersistenceRepository>();

        builder.Services.AddScoped<IMessageQueryRepository, MessageQueryRepository>();
        builder.Services.AddScoped<IMessagePersistenceRepository, MessagePersistenceRepository>();
        builder.Services.AddScoped<IConnectionQueryRepository, ConnectionQueryRepository>();
        builder.Services.AddScoped<IConnectionPersistenceRepository, ConnectionPersistenceRepository>();
        builder.Services.AddScoped<IConversationQueryRepository, ConversationQueryRepository>();
        builder.Services.AddScoped<IConversationPersistenceRepository, ConversationPersistenceRepository>();
        builder.Services.AddScoped<IGroupQueryRepository, GroupQueryRepository>();
        builder.Services.AddScoped<IGroupPersistenceRepository, GroupPersistenceRepository>();

        builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
        builder.Services.AddScoped<IUserPersistenceRepository, UserPersistenceRepository>();

        builder.Services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingPipelineBehaviour<,>));

        // Transients
        builder.Services.AddTransient<ICloudinaryWebClient, CloudinaryWebClient>();
        builder.Services.AddTransient<ICryptoService, CryptoService>();
        builder.Services.AddTransient<IMessageFactory, MessageFactory>();
        builder.Services.AddTransient<IUserFactory, UserFactory>();
        builder.Services.AddTransient<IMessageNotificationService, MessageNotificationService>();
        builder.Services.AddTransient<IBearerTokenGeneratorService, BearerTokenGeneratorService>();
        builder.Services.AddHttpClient();

        // MediatR
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<AssemblyReference>();
            config.RegisterServicesFromAssemblyContaining<Application.AssemblyReference>();
            config.AddOpenRequestPreProcessor(typeof(ValidationPreProcessor<>));
        });

        // SignalR
        builder.Services.AddSignalR();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }
}
