using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

namespace RecipeSocialMediaAPI.Infrastructure.MongoDocuments;

[MongoCollection("")]
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
