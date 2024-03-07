using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Group")]
public record GroupDocument(
    string GroupName,
    string GroupDescription,
    List<string> UserIds,
    string? Id = null
) : MongoDocument(Id);
