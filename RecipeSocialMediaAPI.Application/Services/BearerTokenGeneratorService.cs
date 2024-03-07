using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeSocialMediaAPI.Application.Identity;
using RecipeSocialMediaAPI.Application.Options;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeSocialMediaAPI.Application.Services;

public class BearerTokenGeneratorService : IBearerTokenGeneratorService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BearerTokenGeneratorService(IOptions<JwtOptions> jwtOptions, IDateTimeProvider dateTimeProvider)
    {
        _jwtOptions = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateToken(IUserCredentials user)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Key);

        List<Claim> claims = new()
        {
            new(ClaimTypes.Name, user.Account.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
            new(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
        };

        if (user.Account.Role is UserRole.Developer)
        {
            claims.Add(new(IdentityData.DeveloperUserClaimName, "true", ClaimValueTypes.Boolean));
        }
        if (user.Account.Role is UserRole.Admin)
        {
            claims.Add(new(IdentityData.AdminUserClaimName, "true", ClaimValueTypes.Boolean));
        }

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeProvider.Now.Add(_jwtOptions.Lifetime).UtcDateTime,
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
}
