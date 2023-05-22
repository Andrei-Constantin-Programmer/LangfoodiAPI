using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public interface IUserTokenService
    {
        UserTokenDto GetTokenFromUser(UserDto user);
        UserTokenDto GenerateToken(UserDto user);
        UserTokenDto GenerateToken(UserDocument user);
        UserDocument GetUserFromTokenWithPassword(string token);
        UserDocument GetUserFromToken(string token);
        bool CheckTokenExists(string token);
        bool CheckTokenExists(UserDto user);
        bool CheckTokenExpired(string token);
        bool CheckTokenExpired(UserDto user);
        bool CheckValidToken(string token);
    }
}
