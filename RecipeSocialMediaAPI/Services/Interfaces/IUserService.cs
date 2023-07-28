namespace RecipeSocialMediaAPI.Core.Services;

internal interface IUserService
{
    bool DoesEmailExist (string email);
    bool DoesUsernameExist (string username);
}
