using RecipeSocialMediaAPI.Domain.Models.Users;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Application.Repositories.Users;

public interface IUserPersistenceRepository
{
    [ExcludeFromCodeCoverage(Justification = "Implementation is tested instead")]
    Task<IUserCredentials> CreateUserAsync(
        string handler,
        string username,
        string email,
        string password,
        DateTimeOffset accountCreationDate,
        UserRole userRole = UserRole.User,
        CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(IUserCredentials user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(IUserCredentials user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
}
