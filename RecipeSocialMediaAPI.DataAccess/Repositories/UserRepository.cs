using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Model;

namespace RecipeSocialMediaAPI.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    private readonly IMongoCollectionWrapper<UserDocument> _userCollection;

    public UserRepository(DatabaseConfiguration databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration;
        _userCollection = new MongoCollectionWrapper<UserDocument>(_databaseConfiguration);
    }

    public IEnumerable<User> GetAllUsers() =>
        _userCollection
            .GetAll((_) => true)
            .Select(UserDocumentToModelMapper.MapUserDocumentToUser);

    public User? GetUserById(string id)
    {
        var userDocument = _userCollection
            .Find(userDoc => userDoc.Id == id);

        return userDocument is null 
            ? null
            : UserDocumentToModelMapper.MapUserDocumentToUser(userDocument);
    }

    public User? GetUserByEmail(string email)
    {
        var userDocument = _userCollection
            .Find(userDoc => userDoc.Email == email);

        return userDocument is null
            ? null
            : UserDocumentToModelMapper.MapUserDocumentToUser(userDocument);
    }

    public User? GetUserByUsername(string username)
    {
        var userDocument = _userCollection
            .Find(userDoc => userDoc.UserName == username);

        return userDocument is null
            ? null
            : UserDocumentToModelMapper.MapUserDocumentToUser(userDocument);
    }

    public User CreateUser(string username, string email, string password)
    {
        UserDocument newUserDocument = new()
        {
            UserName = username,
            Email = email,
            Password = password
        };

        newUserDocument = _userCollection.Insert(newUserDocument);

        return UserDocumentToModelMapper.MapUserDocumentToUser(newUserDocument);
    }

    public bool DeleteUser(User user) => DeleteUser(user.Id);

    public bool DeleteUser(string id) => _userCollection.Delete(userDoc => userDoc.Id == id);

    public bool UpdateUser(User user)
    {
        var userDocument = _userCollection.Find(userDoc => userDoc.Id == user.Id)!;

        userDocument.UserName = user.UserName;
        userDocument.Email = user.Email;
        userDocument.Password = user.Password;

        return _userCollection.UpdateRecord(userDocument, userDoc => userDoc.Id == userDocument.Id);
    }
}
