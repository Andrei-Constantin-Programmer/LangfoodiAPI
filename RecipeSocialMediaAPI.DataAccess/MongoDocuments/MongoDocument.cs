using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("")]
public abstract record MongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string? Id { get; set; }
}
