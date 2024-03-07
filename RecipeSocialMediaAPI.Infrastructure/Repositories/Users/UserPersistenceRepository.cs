using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Repositories.Users;

public class UserPersistenceRepository : IUserPersistenceRepository
{
    private readonly IMongoCollectionWrapper<UserDocument> _userCollection;
    private readonly IUserDocumentToModelMapper _mapper;

    public UserPersistenceRepository(IUserDocumentToModelMapper mapper, IMongoCollectionFactory mongoCollectionFactory)
    {
        _mapper = mapper;
        _userCollection = mongoCollectionFactory.CreateCollection<UserDocument>();
    }

    public async Task<IUserCredentials> CreateUserAsync(
        string handler,
        string username,
        string email,
        string password,
        DateTimeOffset accountCreationDate,
        UserRole userRole = UserRole.User, 
        CancellationToken cancellationToken = default)
    {
        UserDocument newUserDocument = new(handler, username, email, password, (int)userRole, null, accountCreationDate);
        
        newUserDocument = await _userCollection.InsertAsync(newUserDocument, cancellationToken);

        return _mapper.MapUserDocumentToUser(newUserDocument);
    }

    public async Task<bool> UpdateUserAsync(IUserCredentials user, CancellationToken cancellationToken = default)
    {
        var userDocument = await _userCollection.GetOneAsync(userDoc => userDoc.Id == user.Account.Id, cancellationToken);

        if (userDocument is null)
        {
            return false;
        }

        var updatedUserDocument = userDocument with
        {
            UserName = user.Account.UserName,
            Email = user.Email,
            Password = user.Password,
            ProfileImageId = user.Account.ProfileImageId,
            Role = (int)user.Account.Role,
            PinnedConversationIds = user.Account.PinnedConversationIds.ToList(),
            BlockedConnectionIds = user.Account.BlockedConnectionIds.ToList()
        };

        return await _userCollection.UpdateAsync(updatedUserDocument, userDoc => userDoc.Id == userDocument.Id, cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(IUserCredentials user, CancellationToken cancellationToken = default) 
        => await DeleteUserAsync(user.Account.Id, cancellationToken);

    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default) 
        => await _userCollection.DeleteAsync(userDoc => userDoc.Id == id, cancellationToken);
}
