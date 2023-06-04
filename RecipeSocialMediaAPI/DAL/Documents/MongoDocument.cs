using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;

namespace RecipeSocialMediaAPI.DAL.Documents;

[MongoCollection("")]
public abstract record MongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    required public string Id { get; set; }
}
