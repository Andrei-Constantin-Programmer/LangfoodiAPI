using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public class MongoFactory : IMongoFactory
    {
        public IMongoCollectionWrapper<T> GetCollection<T>(IRepository repo, IConfigManager config) where T : MongoDocument
        {
            return new MongoCollectionWrapper<T>(repo, config);
        }
    }
}
