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

    public async Task<TDocument?> GetOneAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default)
    {
        return (await _collection.FindAsync(expr, cancellationToken: cancellationToken)).FirstOrDefault(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TDocument>> GetAllAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default)
    {
        return (await _collection.FindAsync(expr, cancellationToken: cancellationToken))
            .ToEnumerable(cancellationToken: cancellationToken) ?? Enumerable.Empty<TDocument>();
    }

    public async Task<TDocument> InsertAsync(TDocument doc, CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.InsertOneAsync(doc, cancellationToken: cancellationToken);
            return doc;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new DocumentAlreadyExistsException<TDocument>(doc);
        }
    }

    public async Task<bool> UpdateAsync(TDocument record, Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default)
    {
        try
        {
            return (await _collection.ReplaceOneAsync(expr, record, cancellationToken: cancellationToken)).ModifiedCount > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default)
    {
        return (await _collection.DeleteOneAsync(expr, cancellationToken)).DeletedCount > 0;
    }

    private protected static string GetCollectionName(Type documentType) =>
        ((MongoCollectionAttribute)documentType.GetCustomAttributes(typeof(MongoCollectionAttribute), true).First()).CollectionName;
}
