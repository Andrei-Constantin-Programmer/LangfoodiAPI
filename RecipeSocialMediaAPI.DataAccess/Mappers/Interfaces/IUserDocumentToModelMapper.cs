using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;

public interface IUserDocumentToModelMapper
{
    User MapUserDocumentToUser(UserDocument userDocument);
}