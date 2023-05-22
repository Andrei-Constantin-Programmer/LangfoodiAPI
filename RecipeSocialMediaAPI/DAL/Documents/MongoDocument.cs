using MongoDB.Bson;

namespace RecipeSocialMediaAPI.DAL.Documents
{
    public abstract record MongoDocument
    {
        #pragma warning disable IDE1006 // Naming Styles
        public BsonObjectId? _id { get; set; }
        #pragma warning restore IDE1006 // Naming Styles
    }
}
