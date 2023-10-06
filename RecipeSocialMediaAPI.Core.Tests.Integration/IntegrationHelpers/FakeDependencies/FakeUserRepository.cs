using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeUserRepository : IUserQueryRepository, IUserPersistenceRepository
{
    private readonly List<UserCredentials> _collection;

    public FakeUserRepository()
    {
        _collection = new List<UserCredentials>();
    }

    public UserCredentials CreateUser(string username, string email, string password) 
    {
        var id = _collection.Count.ToString();
        UserCredentials newUser = new(id, username, email, password);
        _collection.Add(newUser);

        return newUser;
    }

    public bool DeleteUser(UserCredentials user) => DeleteUser(user.Id);

    public bool DeleteUser(string id) => _collection.RemoveAll(user => user.Id == id) > 0;

    public IEnumerable<UserCredentials> GetAllUsers() => _collection;

    public UserCredentials? GetUserById(string id) => _collection.Find(user => user.Id == id);

    public UserCredentials? GetUserByEmail(string email) => _collection.Find(user => user.Email == email);

    public UserCredentials? GetUserByUsername(string username) => _collection.Find(user => user.UserName == username);

    public bool UpdateUser(UserCredentials user)
    {
        UserCredentials? updatedUser = _collection.FirstOrDefault(u => u.Id == user.Id);
        if(updatedUser is null) 
        {
            return false;
        }

        updatedUser.UserName = user.UserName;
        updatedUser.Email = user.Email;
        updatedUser.Password = user.Password;

        return true;
    }
}
