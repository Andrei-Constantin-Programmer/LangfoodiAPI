using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

public interface IUserRepository
{
    IEnumerable<User> GetAllUsers();
    User? GetUserById(string id);
    User? GetUserByEmail(string email);
    User? GetUserByUsername(string username);
    User CreateUser(string username, string email, string password);
    bool DeleteUser(User user);
    bool DeleteUser(string id);
    bool UpdateUser(User user);
}
