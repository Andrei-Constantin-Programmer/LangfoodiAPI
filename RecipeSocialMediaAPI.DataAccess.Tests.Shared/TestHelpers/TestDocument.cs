using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Shared.TestHelpers;

[MongoCollection("TestDocument")]
public record TestDocument(string? TestProperty, string? Id = null) : MongoDocument(Id);