using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserPersistenceRepository
{
    IUserCredentials CreateUser(string handler, string username, string email, string password, DateTimeOffset accountCreationDate);
    bool DeleteUser(IUserCredentials user);
    bool DeleteUser(string id);
    bool UpdateUser(IUserCredentials user);
}
