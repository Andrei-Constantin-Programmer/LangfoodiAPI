using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public class MongoFactory : IMongoFactory
    {
        public IMongoCollection<T> GetCollection<T>(IRepository repo, IConfigManager config) where T : class
        {
            return new MongoCollection<T>(repo, config);
        }
    }
}
