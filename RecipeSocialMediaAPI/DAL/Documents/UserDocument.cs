using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    public record UserDocument
    {
        public required BsonObjectId _id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
