using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("User")]
public record UserDocument(
    string Handler,
    string UserName,
    string Email,
    string Password,
    int Role,
    string? ProfileImageId = null,
    DateTimeOffset? AccountCreationDate = null,
    string? Id = null,
    List<string>? PinnedConversationIds = null,
    List<string>? BlockedConnectionIds = null
) : MongoDocument(Id);