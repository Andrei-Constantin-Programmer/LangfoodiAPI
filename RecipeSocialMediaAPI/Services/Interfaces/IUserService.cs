using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public interface IUserService
    {
        bool RemoveUser(string token, IUserTokenService userTokenService);
        bool UpdateUser(IUserValidationService validationService, IUserTokenService userTokenService, string token, UserDto user);
    }
}
