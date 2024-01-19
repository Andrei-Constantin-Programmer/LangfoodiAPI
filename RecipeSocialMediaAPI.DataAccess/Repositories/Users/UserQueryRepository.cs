using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using Microsoft.Extensions.Logging;

namespace RecipeSocialMediaAPI.DataAccess.Repositories.Users;

public class UserQueryRepository : IUserQueryRepository
{
    private readonly ILogger<UserQueryRepository> _logger;
    private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
    private readonly IUserDocumentToModelMapper _mapper;

    public UserQueryRepository(ILogger<UserQueryRepository> logger, IUserDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _logger = logger;
        _mapper = mapper;
        _userCollection = mongoCollectionFactory.CreateCollection<UserDocument>();
    }

    public IEnumerable<IUserCredentials> GetAllUsers() =>
        _userCollection
            .GetAll((_) => true)
            .Select(_mapper.MapUserDocumentToUser);

    public IUserCredentials? GetUserById(string id)
    {
        UserDocument? userDocument;
        try
        {
            userDocument = _userCollection
                .Find(userDoc => userDoc.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was an error trying to get user by id {Id}: {ErrorMessage}", id, ex.Message);
            userDocument = null;
        }

        return userDocument is null
            ? null
            : _mapper.MapUserDocumentToUser(userDocument);
    }

    public IUserCredentials? GetUserByEmail(string email)
    {
        UserDocument? userDocument;
        try
        {
            userDocument = _userCollection
                .Find(userDoc => userDoc.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was an error trying to get user by email {Email}: {ErrorMessage}", email, ex.Message);
            userDocument = null;
        }

        return userDocument is null
            ? null
            : _mapper.MapUserDocumentToUser(userDocument);
    }

    public IUserCredentials? GetUserByHandler(string handler)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = _userCollection
                .Find(userDoc => userDoc.Handler == handler);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was an error trying to get user by handler {Handler}: {ErrorMessage}", handler, ex.Message);
            userDocument = null;
        }

        return userDocument is null
            ? null
            : _mapper.MapUserDocumentToUser(userDocument);
    }

    public IUserCredentials? GetUserByUsername(string username)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = _userCollection
                .Find(userDoc => userDoc.UserName == username);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "There was an error trying to get user by username {Username}: {ErrorMessage}", username, ex.Message);
            userDocument = null;
        }

        return userDocument is null
            ? null
            : _mapper.MapUserDocumentToUser(userDocument);
    }
}
