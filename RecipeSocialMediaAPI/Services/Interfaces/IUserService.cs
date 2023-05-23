using RecipeSocialMediaAPI.Data.DTO;

namespace RecipeSocialMediaAPI.Services
{
    public interface IUserService
    {
        bool DoesUserExist(UserDto user);
    }
}