using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public interface IMongoFactory
    {
        IMongoCollectionManager<T> GetCollectionManager<T>(IRepository repo, IConfigManager config) where T : class;
    }
}
