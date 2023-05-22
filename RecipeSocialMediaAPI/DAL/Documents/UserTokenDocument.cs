using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    public record UserTokenDocument : MongoDocument
    {
#pragma warning disable IDE1006 // Naming Styles
        public BsonObjectId? _id { get; init; }
#pragma warning restore IDE1006 // Naming Styles

        public required BsonObjectId UserId { get; init; }
        public required DateTimeOffset ExpiryDate { get; init; }
    }
}
