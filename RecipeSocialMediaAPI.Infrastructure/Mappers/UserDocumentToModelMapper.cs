using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers;

public class UserDocumentToModelMapper : IUserDocumentToModelMapper
{
    private readonly IUserFactory _userFactory;
    private readonly IDataCryptoService _dataCryptoService;

    public UserDocumentToModelMapper(IUserFactory userFactory, IDataCryptoService dataCryptoService)
    {
        _userFactory = userFactory;
        _dataCryptoService = dataCryptoService;
    }

    public IUserCredentials MapUserDocumentToUser(UserDocument userDocument)
    {
        if (userDocument.Id is null)
        {
            throw new ArgumentException("Cannot map User Document with null ID to User");
        }

        return _userFactory.CreateUserCredentials(
            userDocument.Id, 
            _dataCryptoService.Decrypt(userDocument.Handler), 
            _dataCryptoService.Decrypt(userDocument.UserName), 
            _dataCryptoService.Decrypt(userDocument.Email), 
            _dataCryptoService.Decrypt(userDocument.Password),
            userDocument.ProfileImageId is null ? null : _dataCryptoService.Decrypt(userDocument.ProfileImageId),
            userDocument.AccountCreationDate,
            userDocument.PinnedConversationIds,
            userDocument.BlockedConnectionIds,
            (UserRole)userDocument.Role
        );
    }
}
