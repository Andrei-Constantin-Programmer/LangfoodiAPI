using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    public record UserTokenDocument
    {
        public BsonObjectId? TokenId { get; init; }
        public required BsonObjectId UserId { get; init; }
        public required DateTimeOffset ExpiryDate { get; init; }
    }
}
