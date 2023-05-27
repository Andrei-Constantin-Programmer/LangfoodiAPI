using MongoDB.Bson;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    [MongoCollection("")]
    public abstract record MongoDocument
    {
        #pragma warning disable IDE1006 // Naming Styles
        public BsonObjectId? _id { get; set; }
        #pragma warning restore IDE1006 // Naming Styles
    }
}
