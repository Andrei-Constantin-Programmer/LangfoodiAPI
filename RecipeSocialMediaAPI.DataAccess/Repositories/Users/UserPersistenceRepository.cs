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

    public IUserCredentials CreateUser(string handler, string username, string email, string password, DateTimeOffset accountCreationDate)
    {
        UserDocument newUserDocument = new(handler, username, email, password, null, accountCreationDate);
        
        newUserDocument = _userCollection.Insert(newUserDocument);

        return _mapper.MapUserDocumentToUser(newUserDocument);
    }

    public bool DeleteUser(IUserCredentials user) => DeleteUser(user.Account.Id);

    public bool DeleteUser(string id) => _userCollection.Delete(userDoc => userDoc.Id == id);

    public bool UpdateUser(IUserCredentials user)
    {
        var userDocument = _userCollection.Find(userDoc => userDoc.Id == user.Account.Id);

        if (userDocument is null)
        {
            return false;
        }

        var updatedUserDocument = userDocument with
        {
            UserName = user.Account.UserName,
            Email = user.Email,
            Password = user.Password,
        };

        return _userCollection.UpdateRecord(updatedUserDocument, userDoc => userDoc.Id == userDocument.Id);
    }
}
