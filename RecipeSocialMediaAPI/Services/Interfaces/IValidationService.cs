using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public interface IValidationService
    {
        bool ValidPassword(string password);
        bool ValidEmail(string email);
        bool ValidUserName(string userName);
        bool ValidUser(UserDto user, IUserService userService);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
