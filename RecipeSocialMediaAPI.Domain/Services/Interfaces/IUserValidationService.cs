namespace RecipeSocialMediaAPI.Domain.Services.Interfaces;

public interface IUserValidationService
{
    bool ValidPassword(string password);
    bool ValidEmail(string email);
    bool ValidUserName(string userName);
    bool ValidHandler(string handler);
}
