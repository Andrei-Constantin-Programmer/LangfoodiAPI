using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("Connection")]
public record ConnectionDocument(string AccountId1, string AccountId2, string ConnectionStatus, string? Id = null) : MongoDocument(Id);