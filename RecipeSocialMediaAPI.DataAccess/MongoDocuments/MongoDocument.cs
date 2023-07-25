using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("RecipeSocialMediaAPI.DataAccess.Tests.Integration")]

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("")]
public abstract record MongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
}
