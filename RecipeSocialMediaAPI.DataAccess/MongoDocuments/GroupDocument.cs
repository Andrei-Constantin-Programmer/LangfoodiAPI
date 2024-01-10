using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Group")]
public record GroupDocument : MongoDocument
{
    required public string GroupName { get; set; }
    required public string GroupDescription { get; set; }
    required public List<string> UserIds { get; set; }
}
