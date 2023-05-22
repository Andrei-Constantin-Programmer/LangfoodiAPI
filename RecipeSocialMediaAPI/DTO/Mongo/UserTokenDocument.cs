using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DTO.Mongo
{
    public record UserTokenDocument
    {
        public BsonObjectId? _id { get; init; }
        public required BsonObjectId UserId { get; init; }
        public required DateTime ExpiryDate { get; init; }
    }
}
