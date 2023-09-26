using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserQueryRepository
{
    IEnumerable<User> GetAllUsers();
    User? GetUserById(string id);
    User? GetUserByEmail(string email);
    User? GetUserByUsername(string username);
}
