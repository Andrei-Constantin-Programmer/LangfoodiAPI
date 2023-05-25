using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    internal interface IUserValidationService
    {
        bool ValidPassword(string password);
        bool ValidEmail(string email);
        bool ValidUserName(string userName);
        bool ValidUser(UserDto user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
