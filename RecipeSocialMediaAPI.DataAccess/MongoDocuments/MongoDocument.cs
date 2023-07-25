using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("RecipeSocialMediaAPI.DataAccess.Tests.Unit")]
[assembly:InternalsVisibleTo("RecipeSocialMediaAPI.DataAccess.Tests.Integration")]
[assembly:InternalsVisibleTo("RecipeSocialMediaAPI.DataAccess.Tests.Shared")]

namespace RecipeSocialMediaAPI.DataAccess.MongoDocuments;

[MongoCollection("")]
public abstract record MongoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
}
