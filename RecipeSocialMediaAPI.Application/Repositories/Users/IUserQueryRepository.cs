using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserQueryRepository
{
    Task<IEnumerable<IUserCredentials>> GetAllUsers(CancellationToken cancellationToken = default);
    IUserCredentials? GetUserById(string id);
    IUserCredentials? GetUserByHandler(string handler);
    IUserCredentials? GetUserByEmail(string email);
    IUserCredentials? GetUserByUsername(string username);
    Task<IEnumerable<IUserAccount>> GetAllUserAccountsContaining(string containedString, CancellationToken cancellationToken = default);
}
