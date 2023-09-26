using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Users;

public class UserPersistenceRepository : IUserPersistenceRepository
{
    private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
    private readonly IUserDocumentToModelMapper _mapper;

    public UserPersistenceRepository(IUserDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _userCollection = mongoCollectionFactory.CreateCollection<UserDocument>();
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

        return _mapper.MapUserDocumentToUser(newUserDocument);
    }

    public bool DeleteUser(User user) => DeleteUser(user.Id);

    public bool DeleteUser(string id) => _userCollection.Delete(userDoc => userDoc.Id == id);

    public bool UpdateUser(User user)
    {
        var userDocument = _userCollection.Find(userDoc => userDoc.Id == user.Id);

        if (userDocument is null)
        {
            return false;
        }

        userDocument.UserName = user.UserName;
        userDocument.Email = user.Email;
        userDocument.Password = user.Password;

        return _userCollection.UpdateRecord(userDocument, userDoc => userDoc.Id == userDocument.Id);
    }
}
