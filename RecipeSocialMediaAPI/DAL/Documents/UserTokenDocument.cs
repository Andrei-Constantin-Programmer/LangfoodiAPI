using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    [MongoCollection("UserToken")]
    public record UserTokenDocument : MongoDocument
    {
        public required BsonObjectId? UserId { get; init; }
        public required DateTimeOffset ExpiryDate { get; init; }
    }
}
