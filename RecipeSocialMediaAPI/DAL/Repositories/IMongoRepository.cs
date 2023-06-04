using System.Linq.Expressions;
using RecipeSocialMediaAPI.DAL.Documents;

namespace RecipeSocialMediaAPI.DAL.Repositories
{
    internal interface IMongoRepository<TDocument> where TDocument : MongoDocument
    {
        List<TDocument> GetAll(Expression<Func<TDocument, bool>> expr);
        TDocument Insert(TDocument doc);
        bool Contains(Expression<Func<TDocument, bool>> expr);
        TDocument? Find(Expression<Func<TDocument, bool>> expr);
        bool Delete(Expression<Func<TDocument, bool>> expr);
        bool UpdateRecord(TDocument record, Expression<Func<TDocument, bool>> expr);
    }
}
