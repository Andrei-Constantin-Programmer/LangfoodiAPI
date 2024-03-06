using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Infrastructure.Mappers.Interfaces;

public interface IUserDocumentToModelMapper
{
    IUserCredentials MapUserDocumentToUser(UserDocument userDocument);
}