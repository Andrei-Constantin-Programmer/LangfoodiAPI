using System.Linq.Expressions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.Repositories;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL
{
    public class MongoCollectionWrapper<T> : IMongoCollectionWrapper<T> where T : MongoDocument
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly MongoDB.Driver.IMongoCollection<T> _collection;

        public MongoCollectionWrapper(IRepository repo, IConfigManager configManager) 
        {
            _mongoClient = new MongoClient(configManager.GetMongoSetting("Connection"));
            _database = _mongoClient.GetDatabase(configManager.GetMongoSetting("ClusterName"));
            _collection = _database.GetCollection<T>(repo.CollectionName);
        }

        public List<T> QueryCollection(Expression<Func<T, bool>> expr)
        {
            return _collection?.Find(expr).ToList() ?? new List<T>();
        }

        public T Insert(T doc)
        {
            _collection?.InsertOne(doc);
            return doc;
        }

        public bool Delete(Expression<Func<T, bool>> expr)
        {
            return _collection?.DeleteOne(expr).DeletedCount > 0;
        }

        public bool Contains(Expression<Func<T, bool>> expr)
        {
            return _collection?.Find(expr).Any() ?? false;
        }

        public T? Find(Expression<Func<T, bool>> expr)
        {
            return _collection?.Find(expr).FirstOrDefault();
        }

        public bool UpdateRecord(T record, Expression<Func<T, bool>> expr)
        {
            return _collection?.ReplaceOne(expr, record).ModifiedCount > 0;
        }
    }
}
