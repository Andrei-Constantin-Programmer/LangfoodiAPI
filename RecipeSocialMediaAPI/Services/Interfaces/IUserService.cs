using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    internal interface IUserService
    {
        bool DoesEmailExist (string email);
        bool DoesUsernameExist (string username);
    }
}
