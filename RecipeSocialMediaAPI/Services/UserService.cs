using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.DAL.Repositories;

namespace RecipeSocialMediaAPI.Services;

internal class UserService : IUserService
{
    private readonly IMongoRepository<UserDocument> _userCollection;

    public UserService(IMongoCollectionFactory collectionFactory)
    {
        _userCollection = collectionFactory.GetCollection<UserDocument>();
    }

    public bool DoesEmailExist(string email) =>
        _userCollection
            .Contains(user => user.Email.ToLower() == email.ToLower());

    public bool DoesUsernameExist(string username) =>
        _userCollection
            .Contains(user => user.UserName == username);
}
