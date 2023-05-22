using Microsoft.OpenApi.Models;
using RecipeSocialMediaAPI.DAL;
using RecipeSocialMediaAPI.Endpoints;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Utilities;
using Serilog;
using RecipeSocialMediaAPI.Services.Interfaces;
using RecipeSocialMediaAPI.DAL.Repositories;
using AutoMapper;
using RecipeSocialMediaAPI.Mapper.Profiles;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogConfiguration.ConfigureSerilog);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
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
    c.AddSecurityRequirement(requirement);

});

builder.Host.UseSerilog(SerilogConfiguration.ConfigureSerilog);

builder.Services.AddSingleton<IMongoFactory, MongoFactory>();
builder.Services.AddSingleton<IConfigManager, ConfigManager>();
builder.Services.AddSingleton<IClock, SystemClock>();

builder.Services.AddAutoMapper(typeof(UserMappingProfile), typeof(UserTokenMappingProfile));

// Setup business logic services
builder.Services.AddSingleton<IValidationService, ValidationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserTokenService, UserTokenService>();

builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Setup endpoints
app.MapUserEndpoints();
app.MapUserTokenEndpoints();
app.MapRecipeEndpoints();

app.MapPost("/logtest", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello World");
});

app.Run();

public partial class Program { }