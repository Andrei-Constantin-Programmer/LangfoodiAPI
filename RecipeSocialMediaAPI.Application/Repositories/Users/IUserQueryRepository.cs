using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserQueryRepository
{
    Task<IUserCredentials?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IUserCredentials?> GetUserByHandlerAsync(string handler, CancellationToken cancellationToken = default);
    Task<IUserCredentials?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IUserCredentials?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<IUserAccount>> GetAllUserAccountsContainingAsync(string containedString, CancellationToken cancellationToken = default);
}
