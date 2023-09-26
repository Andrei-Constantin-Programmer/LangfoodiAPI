using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Users;

public class UserQueryRepository : IUserQueryRepository
{
    private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
    private readonly IUserDocumentToModelMapper _mapper;

    public UserQueryRepository(IUserDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _userCollection = mongoCollectionFactory.CreateCollection<UserDocument>();
    }

    public IEnumerable<User> GetAllUsers() =>
        _userCollection
            .GetAll((_) => true)
            .Select(_mapper.MapUserDocumentToUser);

    public User? GetUserById(string id)
    {
        var userDocument = _userCollection
            .Find(userDoc => userDoc.Id == id);

        return userDocument is null
            ? null
            : _mapper.MapUserDocumentToUser(userDocument);
    }

    public User? GetUserByEmail(string email)
    {
        var userDocument = _userCollection
            .Find(userDoc => userDoc.Email == email);

        return userDocument is null
            ? null
            : _mapper.MapUserDocumentToUser(userDocument);
    }

    public User? GetUserByUsername(string username)
    {
        var userDocument = _userCollection
            .Find(userDoc => userDoc.UserName == username);

        return userDocument is null
            ? null
            : _mapper.MapUserDocumentToUser(userDocument);
    }
}
