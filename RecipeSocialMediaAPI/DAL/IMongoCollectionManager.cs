using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DAL
{
    public interface IMongoCollectionManager<T> where T : class
    {
        List<T> QueryCollection(Expression<Func<T, bool>> expr);
    }
}
