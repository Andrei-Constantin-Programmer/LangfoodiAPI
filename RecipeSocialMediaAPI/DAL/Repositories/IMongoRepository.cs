using RecipeSocialMediaAPI.DAL.Documents;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DAL.Repositories
{
    public interface IMongoRepository<TDocument> where TDocument : MongoDocument
    {
        List<TDocument> GetAll(Expression<Func<TDocument, bool>> expr);
        TDocument Insert(TDocument doc);
        bool Contains(Expression<Func<TDocument, bool>> expr);
        TDocument? Find(Expression<Func<TDocument, bool>> expr);
        bool Delete(Expression<Func<TDocument, bool>> expr);
        bool UpdateRecord(TDocument record, Expression<Func<TDocument, bool>> expr);
    }
}
