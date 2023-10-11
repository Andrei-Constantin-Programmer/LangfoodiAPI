using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserQueryRepository
{
    IEnumerable<IUserCredentials> GetAllUsers();
    IUserCredentials? GetUserById(string id);
    IUserCredentials? GetUserByEmail(string email);
    IUserCredentials? GetUserByUsername(string username);
}
