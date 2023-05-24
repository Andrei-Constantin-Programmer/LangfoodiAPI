using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL.MongoConfiguration
{
    public class MongoCollectionFactory : IMongoCollectionFactory
    {
        private readonly IConfigManager _configManager;

        public MongoCollectionFactory(IConfigManager configManager) 
        {
            _configManager = configManager;
        }

        public IMongoRepository<TDocument> GetCollection<TDocument>() where TDocument : MongoDocument
        {
            return new MongoRepository<TDocument>(_configManager);
        }
    }
}
