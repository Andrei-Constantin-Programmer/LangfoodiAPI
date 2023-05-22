using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public interface IUserService
    {
        bool CheckEmailExists(UserDto user);
        bool CheckUserNameExists(UserDto user);
        UserTokenDto AddUser(UserDto user, IUserTokenService userTokenService, IUserValidationService validationService);
        bool RemoveUser(string token, IUserTokenService userTokenService);
        bool UpdateUser(IUserValidationService validationService, IUserTokenService userTokenService, string token, UserDto user);
    }
}
