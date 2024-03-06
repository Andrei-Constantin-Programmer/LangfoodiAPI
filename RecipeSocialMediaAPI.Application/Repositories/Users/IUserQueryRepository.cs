using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserQueryRepository
{
    Task<IEnumerable<IUserCredentials>> GetAllUsers(CancellationToken cancellationToken = default);
    Task<IUserCredentials?> GetUserById(string id, CancellationToken cancellationToken = default);
    Task<IUserCredentials?> GetUserByHandler(string handler, CancellationToken cancellationToken = default);
    Task<IUserCredentials?> GetUserByEmail(string email, CancellationToken cancellationToken = default);
    Task<IUserCredentials?> GetUserByUsername(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<IUserAccount>> GetAllUserAccountsContaining(string containedString, CancellationToken cancellationToken = default);
}
