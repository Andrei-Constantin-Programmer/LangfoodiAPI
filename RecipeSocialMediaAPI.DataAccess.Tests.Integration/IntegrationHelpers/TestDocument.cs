using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Integration.IntegrationHelpers;

[MongoCollection("TestDocument")]
public record TestDocument : MongoDocument
{
    public string? TestProperty { get; set; }
}
