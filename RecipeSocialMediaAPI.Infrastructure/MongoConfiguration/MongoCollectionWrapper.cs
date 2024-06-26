﻿using MongoDB.Driver;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Helpers;
using RecipeSocialMediaAPI.Infrastructure.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.Infrastructure.MongoConfiguration;

public class MongoCollectionWrapper<TDocument> : IMongoCollectionWrapper<TDocument> where TDocument : MongoDocument
{
    private readonly IMongoCollection<TDocument> _collection;

    public MongoCollectionWrapper(MongoDatabaseOptions databaseConfiguration)
    {
        var mongoClient = new MongoClient(databaseConfiguration.ConnectionString);
        var database = mongoClient.GetDatabase(databaseConfiguration.ClusterName);
        _collection = database.GetCollection<TDocument>(MongoCollectionWrapper<TDocument>.GetCollectionName(typeof(TDocument)));
    }

    public async Task<TDocument?> GetOneAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default)
    {
        return await (await _collection.FindAsync(expr, cancellationToken: cancellationToken)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
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
        return (await _collection.ReplaceOneAsync(expr, record, cancellationToken: cancellationToken)).ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default)
    {
        return (await _collection.DeleteOneAsync(expr, cancellationToken)).DeletedCount > 0;
    }

    private protected static string GetCollectionName(Type documentType) =>
        ((MongoCollectionAttribute)documentType.GetCustomAttributes(typeof(MongoCollectionAttribute), true)[0]).CollectionName;
}
