using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Entities;

namespace RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;

public interface IUserDocumentToModelMapper
{
    User MapUserDocumentToUser(UserDocument userDocument);
}