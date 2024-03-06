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

    public async Task<IUserCredentials> CreateUser(
        string handler,
        string username,
        string email,
        string password,
        DateTimeOffset accountCreationDate,
        UserRole userRole = UserRole.User, 
        CancellationToken cancellationToken = default)
    {
        UserDocument newUserDocument = new(handler, username, email, password, (int)userRole, null, accountCreationDate);
        
        newUserDocument = await _userCollection.Insert(newUserDocument, cancellationToken);

        return _mapper.MapUserDocumentToUser(newUserDocument);
    }

    public async Task<bool> UpdateUser(IUserCredentials user, CancellationToken cancellationToken = default)
    {
        var userDocument = await _userCollection.Find(userDoc => userDoc.Id == user.Account.Id, cancellationToken);

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

        return await _userCollection.UpdateRecord(updatedUserDocument, userDoc => userDoc.Id == userDocument.Id, cancellationToken);
    }

    public async Task<bool> DeleteUser(IUserCredentials user, CancellationToken cancellationToken = default) 
        => await DeleteUser(user.Account.Id, cancellationToken);

    public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken = default) 
        => await _userCollection.Delete(userDoc => userDoc.Id == id, cancellationToken);
}
