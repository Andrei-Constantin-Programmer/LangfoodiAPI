using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("")]
[ExcludeFromCodeCoverage(Justification = "Unnecessary testing on DTO")]
public abstract record MongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; internal set; } = null;

    protected MongoDocument(string? id)
    {
        Id = id;
    }
}
