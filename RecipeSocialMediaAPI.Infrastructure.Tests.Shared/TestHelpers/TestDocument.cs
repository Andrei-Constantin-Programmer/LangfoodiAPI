using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Shared.TestHelpers;

[MongoCollection("TestDocument")]
public record TestDocument(string? TestProperty, string? Id = null) : MongoDocument(Id);