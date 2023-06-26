using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services;

public interface IUserValidationService
{
    bool ValidPassword(string password);
    bool ValidEmail(string email);
    bool ValidUserName(string userName);
    bool ValidUser(NewUserDTO user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
