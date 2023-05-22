using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    public record UserTokenDocument : MongoDocument
    {
        public required BsonObjectId? UserId { get; init; }
        public required DateTimeOffset ExpiryDate { get; init; }
    }
}
