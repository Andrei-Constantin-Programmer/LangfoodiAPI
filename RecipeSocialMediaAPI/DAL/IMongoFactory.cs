using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public interface IMongoFactory
    {
        IMongoCollectionWrapper<T> GetCollection<T>(IRepository repo, IConfigManager config) where T : MongoDocument;
    }
}
