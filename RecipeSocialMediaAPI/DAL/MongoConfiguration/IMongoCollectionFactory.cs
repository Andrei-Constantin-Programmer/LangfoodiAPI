using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;

namespace RecipeSocialMediaAPI.DAL.MongoConfiguration
{
    internal interface IMongoCollectionFactory
    {
        IMongoRepository<TDocument> GetCollection<TDocument>() where TDocument : MongoDocument;
    }
}
