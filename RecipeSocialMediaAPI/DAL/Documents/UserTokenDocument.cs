using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    [MongoCollection("UserToken")]
    public record UserTokenDocument : MongoDocument
    {
        required public BsonObjectId? UserId { get; init; }
        required public DateTimeOffset ExpiryDate { get; init; }
    }
}
