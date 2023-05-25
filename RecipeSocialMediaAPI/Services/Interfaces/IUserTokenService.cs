using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    internal interface IUserTokenService
    {
        UserDocument GetUserFromTokenWithPassword(string token);
        UserDocument GetUserFromToken(string token);
        bool CheckTokenExists(string token);
        bool CheckTokenExists(UserDto user);
        bool CheckTokenExpired(string token);
        bool CheckTokenExpired(UserDto user);
        bool CheckTokenExistsAndNotExpired(string token);
    }
}
