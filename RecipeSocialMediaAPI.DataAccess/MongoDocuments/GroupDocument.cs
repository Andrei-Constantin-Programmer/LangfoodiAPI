using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Group")]
public record GroupDocument(string GroupName, string GroupDescription, List<string> UserIds, string? Id = null) : MongoDocument(Id);
