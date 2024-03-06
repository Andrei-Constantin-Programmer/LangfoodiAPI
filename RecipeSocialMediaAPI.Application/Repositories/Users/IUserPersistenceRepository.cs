using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserPersistenceRepository
{
    Task<IUserCredentials> CreateUser(
        string handler,
        string username,
        string email,
        string password,
        DateTimeOffset accountCreationDate,
        UserRole userRole = UserRole.User,
        CancellationToken cancellationToken = default);
    Task<bool> UpdateUser(IUserCredentials user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUser(IUserCredentials user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUser(string id, CancellationToken cancellationToken = default);
}
