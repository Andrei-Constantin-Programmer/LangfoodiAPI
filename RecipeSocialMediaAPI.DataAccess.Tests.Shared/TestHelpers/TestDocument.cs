using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Shared.TestHelpers;

[MongoCollection("TestDocument")]
public record TestDocument : MongoDocument
{
    public string? TestProperty { get; set; }
}
