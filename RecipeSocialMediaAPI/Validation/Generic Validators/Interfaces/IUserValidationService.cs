namespace RecipeSocialMediaAPI.Validation.GenericValidators.Interfaces;

public interface IUserValidationService
{
    bool ValidPassword(string password);
    bool ValidEmail(string email);
    bool ValidUserName(string userName);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
