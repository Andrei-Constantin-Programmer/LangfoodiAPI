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

    public async Task<IEnumerable<IUserCredentials>> GetAllUsers(CancellationToken cancellationToken = default) => (await _userCollection
        .GetAll((_) => true, cancellationToken))
        .Select(_mapper.MapUserDocumentToUser);

    public async Task<IUserCredentials?> GetUserById(string id, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = await _userCollection
                .Find(userDoc => userDoc.Id == id, cancellationToken);
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

    public async Task<IUserCredentials?> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;
        try
        {
            userDocument = await _userCollection
                .Find(userDoc => userDoc.Email == email, cancellationToken);
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

    public async Task<IUserCredentials?> GetUserByHandler(string handler, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = await _userCollection
                .Find(userDoc => userDoc.Handler == handler, cancellationToken);
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

    public async Task<IUserCredentials?> GetUserByUsername(string username, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = await _userCollection
                .Find(userDoc => userDoc.UserName == username, cancellationToken);
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

    public async Task<IEnumerable<IUserAccount>> GetAllUserAccountsContaining(string containedString, CancellationToken cancellationToken = default) => 
        (await _userCollection
            .GetAll(userDoc => userDoc.Handler.Contains(containedString.ToLower())
                            || userDoc.UserName.Contains(containedString.ToLower()), cancellationToken))
            .Select(userDoc => _mapper.MapUserDocumentToUser(userDoc).Account);
}
