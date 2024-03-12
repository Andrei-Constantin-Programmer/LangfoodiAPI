using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Users;

public class UserQueryRepository : IUserQueryRepository
{
    private readonly ILogger<UserQueryRepository> _logger;
    private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
    private readonly IUserDocumentToModelMapper _mapper;
    private readonly IDataCryptoService _dataCryptoService;

    public UserQueryRepository(
        ILogger<UserQueryRepository> logger,
        IUserDocumentToModelMapper mapper,
        IMongoCollectionFactory mongoCollectionFactory,
        IDataCryptoService dataCryptoService)
    {
        _logger = logger;
        _mapper = mapper;
        _userCollection = mongoCollectionFactory.CreateCollection<UserDocument>();
        _dataCryptoService = dataCryptoService;
    }

    public async Task<IUserCredentials?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = await _userCollection
                .GetOneAsync(userDoc => userDoc.Id == id, cancellationToken);
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

    public async Task<IUserCredentials?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;
        try
        {
            userDocument = (await _userCollection.GetAllAsync(_ => true, cancellationToken))
                .FirstOrDefault(userDoc => _dataCryptoService.Decrypt(userDoc.Email) == email);
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

    public async Task<IUserCredentials?> GetUserByHandlerAsync(string handler, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = (await _userCollection.GetAllAsync(_ => true, cancellationToken))
                .FirstOrDefault(userDoc => _dataCryptoService.Decrypt(userDoc.Handler) == handler);
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

    public async Task<IUserCredentials?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        UserDocument? userDocument;

        try
        {
            userDocument = (await _userCollection.GetAllAsync(_ => true, cancellationToken))
                .FirstOrDefault(userDoc => _dataCryptoService.Decrypt(userDoc.UserName) == username);
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

    public async Task<IEnumerable<IUserAccount>> GetAllUserAccountsContainingAsync(string containedString, CancellationToken cancellationToken = default)
        => (await _userCollection
            .GetAllAsync((_ => true), cancellationToken))
            .Where(userDoc => _dataCryptoService.Decrypt(userDoc.Handler).Contains(containedString.ToLower())
                           || _dataCryptoService.Decrypt(userDoc.UserName).Contains(containedString.ToLower()))
            .Select(userDoc => {
                try
                {
                    return _mapper.MapUserDocumentToUser(userDoc).Account;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was an error trying to map user {UserId}", userDoc.Id);
                    return null;
                }
            })
        .OfType<IUserAccount>();
}
