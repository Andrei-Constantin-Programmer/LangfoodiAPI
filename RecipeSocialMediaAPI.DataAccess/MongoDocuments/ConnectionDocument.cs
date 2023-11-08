using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Connection")]
public record ConnectionDocument : MongoDocument
{
    required public string AccountId1 { get; set; }
    required public string AccountId2 { get; set; }
    required public string ConnectionStatus { get; set; }
}
