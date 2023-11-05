using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Group")]
public record GroupDocument : MongoDocument
{
    required public List<string> AccountIds { get; set; }
    required public string ConnectionStatus { get; set; }
}
