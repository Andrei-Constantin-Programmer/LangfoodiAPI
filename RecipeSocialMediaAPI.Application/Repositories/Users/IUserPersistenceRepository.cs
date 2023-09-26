using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserPersistenceRepository
{
    User CreateUser(string username, string email, string password);
    bool DeleteUser(User user);
    bool DeleteUser(string id);
    bool UpdateUser(User user);
}
