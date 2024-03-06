using System.Linq.Expressions;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;

public interface IMongoCollectionWrapper<TDocument> where TDocument : MongoDocument
{
    Task<IEnumerable<TDocument>> GetAll(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    TDocument Insert(TDocument doc);
    bool Contains(Expression<Func<TDocument, bool>> expr);
    Task<TDocument?> Find(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    bool Delete(Expression<Func<TDocument, bool>> expr);
    bool UpdateRecord(TDocument record, Expression<Func<TDocument, bool>> expr);
}
