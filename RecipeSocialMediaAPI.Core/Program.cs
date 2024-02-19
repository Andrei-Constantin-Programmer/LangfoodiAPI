using RecipeSocialMediaAPI.Core.Utilities;
using RecipeSocialMediaAPI.Core.Configuration;
using RecipeSocialMediaAPI.Core.Middleware;
using RecipeSocialMediaAPI.Core.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RecipeSocialMediaAPI.Application.Options;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Services.AddEndpointsApiExplorer();
builder.ConfigureSwagger();

builder.ConfigureOptions();
builder.ConfigureServices();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        JwtOptions settings = builder.Configuration.GetSection(JwtOptions.CONFIGURATION_SECTION).Get<JwtOptions>()!;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = settings.Issuer,
            ValidAudience = settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMappingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MessagingHub>("messaging-hub");
app.UseCors("AllowAll");
app.MapEndpoints();
app.Run();


public partial class Program { }
