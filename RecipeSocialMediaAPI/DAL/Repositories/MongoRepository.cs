using System.Linq.Expressions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DAL.Documents;
using RecipeSocialMediaAPI.DAL.MongoConfiguration;
using RecipeSocialMediaAPI.Utilities;

namespace RecipeSocialMediaAPI.DAL.Repositories;

internal class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : MongoDocument
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TDocument> _collection;

    public MongoRepository(IConfigManager configManager)
    {
        _mongoClient = new MongoClient(configManager.GetMongoSetting("Connection"));
        _database = _mongoClient.GetDatabase(configManager.GetMongoSetting("ClusterName"));
        _collection = _database.GetCollection<TDocument>(MongoRepository<TDocument>.GetCollectionName(typeof(TDocument)));
    }

    public List<TDocument> GetAll(Expression<Func<TDocument, bool>> expr)
    {
        return _collection?.Find(expr).ToList() ?? new List<TDocument>();
    }

    public TDocument Insert(TDocument doc)
    {
        _collection?.InsertOne(doc);
        return doc;
    }

    public bool Delete(Expression<Func<TDocument, bool>> expr)
    {
        return _collection?.DeleteOne(expr).DeletedCount > 0;
    }

    public bool Contains(Expression<Func<TDocument, bool>> expr)
    {
        return _collection?.Find(expr).Any() ?? false;
    }

    public TDocument? Find(Expression<Func<TDocument, bool>> expr)
    {
        return _collection?.Find(expr).FirstOrDefault();
    }

    public bool UpdateRecord(TDocument record, Expression<Func<TDocument, bool>> expr)
    {
        return _collection?.ReplaceOne(expr, record).ModifiedCount > 0;
    }

    private protected static string GetCollectionName(Type documentType) =>
        ((MongoCollectionAttribute)documentType.GetCustomAttributes(typeof(MongoCollectionAttribute), true).First()).CollectionName;
}
