using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeUserRepository : IUserQueryRepository, IUserPersistenceRepository
{
    private readonly List<IUserCredentials> _collection;

    public FakeUserRepository()
    {
        _collection = new List<IUserCredentials>();
    }

    public async Task<IUserCredentials> CreateUserAsync(string handler, string username, string email, string password, DateTimeOffset accountCreationDate, UserRole userRole = UserRole.User, CancellationToken cancellationToken = default)
    {
        var id = _collection.Count.ToString();
        IUserCredentials newUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = id,
                Handler = handler,
                UserName = username,
                AccountCreationDate = accountCreationDate,
                Role = userRole
            },
            Email = email,
            Password = password
        };
        _collection.Add(newUser);

        return await Task.FromResult(newUser);
    }

    public async Task<bool> DeleteUserAsync(IUserCredentials user, CancellationToken cancellationToken = default) 
        => await DeleteUserAsync(user.Account.Id, cancellationToken);

    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.RemoveAll(user => user.Account.Id == id) > 0);

    public async Task<IEnumerable<IUserCredentials>> GetAllUsersAsync(CancellationToken cancellationToken = default) => await Task.FromResult(_collection);

    public async Task<IUserCredentials?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Find(user => user.Account.Id == id));

    public async Task<IUserCredentials?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Find(user => user.Email == email));

    public async Task<IUserCredentials?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Find(user => user.Account.UserName == username));

    public async Task<IUserCredentials?> GetUserByHandlerAsync(string handler, CancellationToken cancellationToken = default) 
        => await Task.FromResult(_collection.Find(user => user.Account.Handler == handler));

    public async Task<bool> UpdateUserAsync(IUserCredentials user, CancellationToken cancellationToken = default)
    {
        IUserCredentials? updatedUser = _collection.FirstOrDefault(u => u.Account.Id == user.Account.Id);
        if (updatedUser is null)
        {
            return false;
        }

        updatedUser.Account.UserName = user.Account.UserName;
        updatedUser.Account.Role = user.Account.Role;
        updatedUser.Email = user.Email;
        updatedUser.Password = user.Password;

        return await Task.FromResult(true);
    }

    public Task<IEnumerable<IUserAccount>> GetAllUserAccountsContainingAsync(string containedString, CancellationToken cancellationToken = default) => Task.FromResult(_collection
        .Where(user => user.Account.Handler.Contains(containedString, StringComparison.InvariantCultureIgnoreCase)
                    || user.Account.UserName.Contains(containedString, StringComparison.InvariantCultureIgnoreCase))
        .Select(user => user.Account));
}
