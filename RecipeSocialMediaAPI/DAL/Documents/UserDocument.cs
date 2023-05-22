using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    public record UserDocument
    {
#pragma warning disable IDE1006 // Naming Styles
        public required BsonObjectId _id { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
