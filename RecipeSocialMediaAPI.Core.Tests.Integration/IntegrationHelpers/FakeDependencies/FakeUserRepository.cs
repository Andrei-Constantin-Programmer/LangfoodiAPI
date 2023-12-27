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

    public IUserCredentials CreateUser(string handler, string username, string email, string password, DateTimeOffset accountCreationDate)
    {
        var id = _collection.Count.ToString();
        IUserCredentials newUser = new TestUserCredentials
        {
            Account = new TestUserAccount
            {
                Id = id,
                Handler = handler,
                UserName = username,
                AccountCreationDate = accountCreationDate
            },
            Email = email,
            Password = password
        };
        _collection.Add(newUser);

        return newUser;
    }

    public bool DeleteUser(IUserCredentials user) => DeleteUser(user.Account.Id);

    public bool DeleteUser(string id) => _collection.RemoveAll(user => user.Account.Id == id) > 0;

    public IEnumerable<IUserCredentials> GetAllUsers() => _collection;

    public IUserCredentials? GetUserById(string id) => _collection.Find(user => user.Account.Id == id);

    public IUserCredentials? GetUserByEmail(string email) => _collection.Find(user => user.Email == email);

    public IUserCredentials? GetUserByUsername(string username) => _collection.Find(user => user.Account.UserName == username);

    public IUserCredentials? GetUserByHandler(string handler) => _collection.Find(user => user.Account.Handler == handler);

    public bool UpdateUser(IUserCredentials user)
    {
        IUserCredentials? updatedUser = _collection.FirstOrDefault(u => u.Account.Id == user.Account.Id);
        if (updatedUser is null)
        {
            return false;
        }

        updatedUser.Account.UserName = user.Account.UserName;
        updatedUser.Email = user.Email;
        updatedUser.Password = user.Password;

        return true;
    }
}
