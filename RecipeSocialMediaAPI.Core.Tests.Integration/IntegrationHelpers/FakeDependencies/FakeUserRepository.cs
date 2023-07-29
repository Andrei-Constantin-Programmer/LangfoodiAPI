using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Model;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers.FakeDependencies;

internal class FakeUserRepository : IUserRepository
{
    private readonly List<User> _collection;

    public FakeUserRepository()
    {
        _collection = new List<User>();
    }

    public User CreateUser(string username, string email, string password) 
    {
        var id = _collection.Count.ToString();
        User newUser = new(id, username, email, password);
        _collection.Add(newUser);

        return newUser;
    }

    public bool DeleteUser(User user) => DeleteUser(user.Id);

    public bool DeleteUser(string id) => _collection.RemoveAll(user => user.Id == id) > 0;

    public IEnumerable<User> GetAllUsers() => _collection;

    public User? GetUserById(string id) => _collection.Find(user => user.Id == id);

    public User? GetUserByEmail(string email) => _collection.Find(user => user.Email == email);

    public User? GetUserByUsername(string username) => _collection.Find(user => user.UserName == username);

    public bool UpdateUser(User user)
    {
        User? updatedUser = _collection.FirstOrDefault(u => u.Id == user.Id);
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
