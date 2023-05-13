using System.Linq.Expressions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public class MongoCollectionManager<T> : IMongoCollectionManager<T> where T : class
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;

        public MongoCollectionManager(IRepository repo, IConfigManager configManager)
        {
            _mongoClient = new MongoClient(configManager.GetMongoSetting("Connection"));
            _database = _mongoClient.GetDatabase(configManager.GetMongoSetting("ClusterName"));
            _collection = _database.GetCollection<T>(repo.CollectionName);
        }

        public List<T> QueryCollection(Expression<Func<T, bool>> expr)
        {
            if (_collection != null) 
            {
                return _collection.Find(expr).ToList();
            }

            return new List<T>();
        }
    }
}
