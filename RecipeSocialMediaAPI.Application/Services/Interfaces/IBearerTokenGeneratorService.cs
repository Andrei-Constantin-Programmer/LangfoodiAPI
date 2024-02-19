using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Services.Interfaces;

public interface IBearerTokenGeneratorService
{
    string GenerateToken(IUserCredentials user);
}