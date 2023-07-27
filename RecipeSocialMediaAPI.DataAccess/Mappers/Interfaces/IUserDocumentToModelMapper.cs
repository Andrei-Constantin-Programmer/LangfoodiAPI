using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Model;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;

public interface IUserDocumentToModelMapper
{
    User MapUserDocumentToUser(UserDocument userDocument);
}