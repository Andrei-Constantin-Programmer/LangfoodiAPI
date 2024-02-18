﻿using System.Linq.Expressions;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;

public interface IMongoCollectionWrapper<TDocument> where TDocument : MongoDocument
{
    IEnumerable<TDocument> GetAll(Expression<Func<TDocument, bool>> expr);
    TDocument Insert(TDocument doc);
    bool Contains(Expression<Func<TDocument, bool>> expr);
    TDocument? Find(Expression<Func<TDocument, bool>> expr);
    bool Delete(Expression<Func<TDocument, bool>> expr);
    bool UpdateRecord(TDocument record, Expression<Func<TDocument, bool>> expr);
}
