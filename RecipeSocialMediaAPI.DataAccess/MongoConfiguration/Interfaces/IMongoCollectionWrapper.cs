using System.Linq.Expressions;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;

namespace RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;

public interface IMongoCollectionWrapper<TDocument> where TDocument : MongoDocument
{
    Task<TDocument?> GetOneAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    Task<IEnumerable<TDocument>> GetAllAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    Task<TDocument> InsertAsync(TDocument doc, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TDocument record, Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Expression<Func<TDocument, bool>> expr, CancellationToken cancellationToken = default);
}
