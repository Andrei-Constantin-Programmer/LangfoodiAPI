using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Connection")]
public record ConnectionDocument(string AccountId1, string AccountId2, string ConnectionStatus, string? Id = null) : MongoDocument(Id);