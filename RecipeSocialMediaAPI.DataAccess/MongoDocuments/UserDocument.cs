using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("User")]
public record UserDocument(
    string Handler,
    string UserName,
    string Email,
    string Password,
    string? ProfileImageId = null,
    DateTimeOffset? AccountCreationDate = null,
    string? Id = null
) : MongoDocument(Id);