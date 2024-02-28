using System.Linq.Expressions;
using MongoDB.Driver;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Helpers;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration;

public class MongoCollectionWrapper<TDocument> : IMongoCollectionWrapper<TDocument> where TDocument : MongoDocument
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TDocument> _collection;

    public MongoCollectionWrapper(MongoDatabaseOptions databaseConfiguration)
    {
        _mongoClient = new MongoClient(databaseConfiguration.ConnectionString);
        _database = _mongoClient.GetDatabase(databaseConfiguration.ClusterName);
        _collection = _database.GetCollection<TDocument>(MongoCollectionWrapper<TDocument>.GetCollectionName(typeof(TDocument)));
    }

    public IEnumerable<TDocument> GetAll(Expression<Func<TDocument, bool>> expr)
    {
        return _collection?.Find(expr).ToEnumerable() ?? Enumerable.Empty<TDocument>();
    }

    public TDocument Insert(TDocument doc)
    {
        try
        {
            _collection?.InsertOne(doc);
            return doc;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new DocumentAlreadyExistsException<TDocument>(doc);
        }
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
        try
        {
            return _collection?.ReplaceOne(expr, record).ModifiedCount > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private protected static string GetCollectionName(Type documentType) =>
        ((MongoCollectionAttribute)documentType.GetCustomAttributes(typeof(MongoCollectionAttribute), true).First()).CollectionName;
}
