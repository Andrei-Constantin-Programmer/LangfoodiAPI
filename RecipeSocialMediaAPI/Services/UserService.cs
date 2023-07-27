using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool DoesEmailExist(string email) =>
        _userRepository.GetUserByEmail(email.ToLower()) is not null;

    public bool DoesUsernameExist(string username) =>
        _userRepository.GetUserByUsername(username) is not null;
}
