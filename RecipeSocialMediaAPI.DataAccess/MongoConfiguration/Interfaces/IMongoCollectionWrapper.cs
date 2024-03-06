using System.Linq.Expressions;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;

public interface IMongoCollectionWrapper<TDocument> where TDocument : MongoDocument
{
    Task<TDocument?> Find(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    Task<IEnumerable<TDocument>> GetAll(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    Task<TDocument> Insert(TDocument doc, CancellationToken cancellationToken = default);
    Task<bool> UpdateRecord(TDocument record, Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    Task<bool> Delete(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
}
