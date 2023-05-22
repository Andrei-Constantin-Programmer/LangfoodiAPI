using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public class MongoFactory : IMongoFactory
    {
        public IMongoCollection<T> GetCollection<T>(IRepository repo, IConfigManager config) where T : MongoDocument
        {
            return new MongoCollection<T>(repo, config);
        }
    }
}
