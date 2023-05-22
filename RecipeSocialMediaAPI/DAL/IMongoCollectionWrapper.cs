using RecipeSocialMediaAPI.DAL.Documents;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DAL
{
    public interface IMongoCollectionWrapper<T> where T : MongoDocument
    {
        List<T> QueryCollection(Expression<Func<T, bool>> expr);
        T Insert(T doc);
        bool Contains(Expression<Func<T, bool>> expr);
        T? Find(Expression<Func<T, bool>> expr);
        bool Delete(Expression<Func<T, bool>> expr);
        bool UpdateRecord(T record, Expression<Func<T, bool>> expr);
    }
}
