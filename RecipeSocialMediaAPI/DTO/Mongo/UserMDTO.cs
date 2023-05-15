using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DTO.Mongo
{
    public record UserMDTO
    {
        public required BsonObjectId _id { get; init; }
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
