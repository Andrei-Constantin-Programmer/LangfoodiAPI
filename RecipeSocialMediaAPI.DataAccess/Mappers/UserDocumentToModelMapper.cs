using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.DataAccess.Mappers;

public class UserDocumentToModelMapper : IUserDocumentToModelMapper
{
    private readonly IUserFactory _userFactory;

    public UserDocumentToModelMapper(IUserFactory userFactory)
    {
        _userFactory = userFactory;
    }

    public IUserCredentials MapUserDocumentToUser(UserDocument userDocument)
    {
        if (userDocument.Id is null)
        {
            throw new ArgumentException("Cannot map User Document with null ID to User");
        }

        return _userFactory.CreateUserCredentials(
            userDocument.Id, userDocument.Handler, userDocument.UserName, 
            userDocument.Email, userDocument.Password, userDocument.ProfileImageId,
            userDocument.AccountCreationDate
        );
    }
}
