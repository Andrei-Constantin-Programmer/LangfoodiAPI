using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserQueryRepository
{
    IEnumerable<IUserCredentials> GetAllUsers();
    IUserCredentials? GetUserById(string id);
    IUserCredentials? GetUserByHandler(string handler);
    IUserCredentials? GetUserByEmail(string email);
    IUserCredentials? GetUserByUsername(string username);
    IEnumerable<IUserAccount> GetAllUserAccountsContaining(string containedString);
}
