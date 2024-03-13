using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IUserDocumentToModelMapper
{
    IUserCredentials MapUserDocumentToUser(UserDocument userDocument);
}