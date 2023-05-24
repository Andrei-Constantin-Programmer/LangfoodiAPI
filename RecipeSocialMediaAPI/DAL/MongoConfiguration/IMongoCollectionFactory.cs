using RecipeSocialMediaAPI.DAL.Documents;

namespace RecipeSocialMediaAPI.DAL.MongoConfiguration
{
    public interface IMongoCollectionFactory
    {
        IMongoRepository<TDocument> GetCollection<TDocument>() where TDocument : MongoDocument;
    }
}
