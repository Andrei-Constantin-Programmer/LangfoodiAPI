using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public interface ITokenGeneratorService
    {
        UserTokenDto GenerateToken(UserDocument user);
        UserTokenDto GenerateToken(UserDto user);
    }
}