using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;

public interface IUserDocumentToModelMapper
{
    IUserCredentials MapUserDocumentToUser(UserDocument userDocument);
}