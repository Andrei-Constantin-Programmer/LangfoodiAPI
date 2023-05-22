using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    public record UserDocument : MongoDocument
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
