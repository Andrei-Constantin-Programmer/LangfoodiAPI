using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public class MongoFactory : IMongoFactory
    {
        public IMongoCollectionManager<T> GetCollectionManager<T>(IRepository repo, IConfigManager config) where T : class
        {
            return new MongoCollectionManager<T>(repo, config);
        }
    }
}
