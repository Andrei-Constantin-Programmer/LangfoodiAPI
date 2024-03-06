using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeSocialMediaAPI.Application.Identity;
using RecipeSocialMediaAPI.Application.Options;
using System.Text;

namespace RecipeSocialMediaAPI.Presentation.Configuration;

public static class AuthConfiguration
{
    public static void ConfigureAuth(this WebApplicationBuilder builder)
    {
        JwtOptions settings = builder.Services.BuildServiceProvider().GetService<IOptions<JwtOptions>>()!.Value;

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
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

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(IdentityData.AdminUserPolicyName,
                policy => policy.RequireClaim(IdentityData.AdminUserClaimName, "true"));
            options.AddPolicy(IdentityData.DeveloperUserPolicyName,
                policy => policy.RequireAssertion(context => context.User.HasClaim(claim =>
                    claim.Type == IdentityData.DeveloperUserClaimName || claim.Type == IdentityData.AdminUserClaimName)));
        });
    }
}
