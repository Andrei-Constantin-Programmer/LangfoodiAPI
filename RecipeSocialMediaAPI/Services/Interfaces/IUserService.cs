using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public interface IUserService
    {
        bool ValidUserLogin(IValidationService validationService, UserDto user);
        bool CheckEmailExists(UserDto user);
        bool CheckUserNameExists(UserDto user);
        UserTokenDto AddUser(UserDto user, IUserTokenService userTokenService, IValidationService validationService);
        bool RemoveUser(string token, IUserTokenService userTokenService);
        bool UpdateUser(IValidationService validationService, IUserTokenService userTokenService, string token, UserDto user);
    }
}
