using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("Connection")]
[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public record ConnectionDocument(string AccountId1, string AccountId2, string ConnectionStatus, string? Id = null) : MongoDocument(Id);