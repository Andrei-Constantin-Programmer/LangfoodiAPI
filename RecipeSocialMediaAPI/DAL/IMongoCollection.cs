using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DAL
{
    public interface IMongoCollection<T> where T : class
    {
        List<T> QueryCollection(Expression<Func<T, bool>> expr);
        T Insert(T doc);
        bool Contains(Expression<Func<T, bool>> expr);
        T? Find(Expression<Func<T, bool>> expr);
        bool Delete(Expression<Func<T, bool>> expr);
        bool UpdateRecord(T record, Expression<Func<T, bool>> expr);
    }
}
